using Newtonsoft.Json.Linq;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using QLDT_Becamex.Src.Shared.Helpers;
using Quartz;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var mandatoryUserIds = (map.GetString("MandatoryUserIds") ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
            // -------- 1) Resolve recipients
            var mandatoryTokens = await _resolver.ResolveByUserIdsAsync(mandatoryUserIds, context.CancellationToken);
            var generalTokens = await _resolver.ResolveByDeptLevelAsync(deptIds, levels, context.CancellationToken);

            // Loại token trùng: ưu tiên nội dung Mandatory
            var mandatorySet = new HashSet<string>(mandatoryTokens.Select(t => t.Token));
            generalTokens = generalTokens.Where(t => !mandatorySet.Contains(t.Token)).ToList();

            // Nếu tất cả đều trống thì kết thúc
            if (mandatoryTokens.Count == 0 && generalTokens.Count == 0) return;

            //---Gửi Mandatory (nếu có)
            if (mandatoryTokens.Count > 0)
            {
                // 1) Soạn payload
                var (title, body, data) = await _notificationComposer.CourseCreated_MandatoryAsync(courseId, context.CancellationToken);

                await SendNotification(title, body, data, mandatoryTokens, context);
            }

            //---Gửi General
            if (generalTokens.Count > 0)
            {
                var (title, body, data) = await _notificationComposer.CourseCreated_GeneralAsync(courseId, context.CancellationToken);

                await SendNotification(title, body, data, generalTokens, context);
            }
        }

        public async Task SendNotification(string title, string body, Dictionary<string, string> data, List<(int DeviceId, string Token)> tokens, IJobExecutionContext context)
        {
            DateTime now = DateTimeHelper.GetVietnamTimeNow();
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

            // 5) Ghi log từng device
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
