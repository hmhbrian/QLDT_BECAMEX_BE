using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using Quartz;
using System.Text.Json;

namespace QLDT_Becamex.Src.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class CourseCreatedNotifyJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFcmSender _fcmSender;
        private readonly INotificationComposer _notificationComposer;
        private readonly IRecipientResolver _resolver;
        private readonly IConfiguration _cfg;

        public CourseCreatedNotifyJob(
            IUnitOfWork unitOfWork, IFcmSender fcmSender, INotificationComposer notificationComposer, IRecipientResolver resolver,
              IConfiguration cfg)
        {
            _unitOfWork = unitOfWork;
            _fcmSender = fcmSender;
            _notificationComposer = notificationComposer;
            _resolver = resolver;
            _cfg = cfg;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.MergedJobDataMap;

            var courseId = map.GetString("CourseId")!;
            var deptIds = (map.GetString("DepartmentIds") ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            var levels = (map.GetString("Levels") ?? "")
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            // 0) Resolve tokens (3 TH: Dept&Level / Dept / Level). Nếu cả 2 rỗng -> không gửi.
            var tokens = await _resolver.ResolveStudentDeviceTokensAsync(deptIds, levels, context.CancellationToken);
            if (tokens.Count == 0)
                return;

            // 1) Soạn payload
            var (title, body, data) = await _notificationComposer.CourseCreatedAsync(courseId, context.CancellationToken);

            var now = DateTime.UtcNow;
            var dataJson = JsonSerializer.Serialize(data);

            //2)Tạo message
            var msg = new Messages
            {
                Title = title,
                Body = body,
                Data = dataJson,
                SendType = "Token",
                SentBy = "System",
                CreatedAt = now 
            };
            await _unitOfWork.MessagesRepository.AddAsync(msg);
            await _unitOfWork.CompleteAsync();

            // 3) Fan-out theo USER -> tạo UserNotifications (một dòng/user)
            //    Lấy các DeviceId -> map sang UserId, rồi Distinct
            var deviceIds = tokens.Select(t => t.DeviceId).Distinct().ToList();

            //Lấy devices theo Ids
            var devices = await _unitOfWork.DevicesRepository.GetFlexibleAsync(
                    predicate: d => deviceIds.Contains(d.Id)
                );
            var userIds = devices.Select(d => d.UserId).Distinct().ToList();

            // chunk để AddRange theo số lượng lớn
            const int chunkSize = 1000;
            foreach (var chunk in userIds.Chunk(chunkSize))
            {
                var rows = chunk.Select(uid => new UserNotification
                {
                    UserId = uid,
                    MessageId = msg.Id,
                    SentAt = now,
                    IsRead = false,
                    ReadAt = null,
                    IsHidden = false
                });

                await _unitOfWork.UserNotificationRepository.AddRangeAsync(rows);
                await _unitOfWork.CompleteAsync();
            }

            // 4) Gửi theo batch (IFcmSender đã tự chunk 500 token/lần)
            var perTokenResults = await _fcmSender.SendMulticastAsync(title, body, data, tokens, context.CancellationToken);

            // 4) Ghi log từng device
            foreach (var r in perTokenResults)
            {
                await _unitOfWork.MessageLogsRepository.AddAsync(new MessageLogs 
                {
                    MessageId = msg.Id,
                    DeviceId = r.DeviceId,
                    TopicId = null,
                    Status = r.Success ? "Sent" : "Failed",
                    ErrorMessage = r.Success ? null : r.Error,
                    SentAt = DateTime.UtcNow
                });

                // token invalid -> có thể đánh Inactive
                if (!r.Success && r.Error is string err && err.Contains("registration-token-not-registered", StringComparison.OrdinalIgnoreCase))
                {
                    var device = await _unitOfWork.DevicesRepository.GetByIdAsync(r.DeviceId);
                    if (device != null)
                        Console.WriteLine(r.DeviceId + "không hoạt động");
                        _unitOfWork.DevicesRepository.Remove(device);
                }
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}
