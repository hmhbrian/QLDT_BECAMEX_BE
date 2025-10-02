using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using Quartz;
using System.Text.Json;
using static QLDT_Becamex.Src.Shared.Helpers.DateTimeHelper;

namespace QLDT_Becamex.Src.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class CourseEndingNotifyJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFcmSender _fcmSender;
        private readonly INotificationComposer _notificationComposer;

        public CourseEndingNotifyJob(
            IUnitOfWork unitOfWork, 
            IFcmSender fcmSender, 
            INotificationComposer notificationComposer)
        {
            _unitOfWork = unitOfWork;
            _fcmSender = fcmSender;
            _notificationComposer = notificationComposer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var map = context.MergedJobDataMap;
            var courseId = map.GetString("CourseId")!;

            // 1) Lấy danh sách UserId đã đăng ký khóa học
            var registeredUserCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                predicate: uc => uc.CourseId == courseId
            );

            if (!registeredUserCourses.Any())
                return;

            var userIds = registeredUserCourses.Select(uc => uc.UserId).Distinct().ToList();

            // 2) Lấy tất cả devices của những user đã đăng ký
            var devices = await _unitOfWork.DevicesRepository.GetFlexibleAsync(
                predicate: d => d.UserId != null && userIds.Contains(d.UserId)
            );

            if (!devices.Any())
                return;

            // 3) Tạo danh sách tokens để gửi FCM
            var tokens = devices
                .Where(d => d.DeviceToken != null)
                .Select(d => (DeviceId: d.Id, Token: d.DeviceToken!))
                .ToList();

            // 4) Soạn payload
            var (title, body, data) = await _notificationComposer.CourseEndingSoonAsync(courseId, context.CancellationToken);

            var now = ToVietnamTime(DateTime.UtcNow);
            Console.WriteLine($"[Send] CourseEndingNotifyJob for course {courseId} to {tokens.Count} devices at {now}");
            var dataJson = JsonSerializer.Serialize(data);

            // 5) Tạo message
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

            // 6) Tạo UserNotifications cho từng user đã đăng ký
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

            // 7) Gửi FCM theo batch (IFcmSender đã tự chunk 500 token/lần)
            var perTokenResults = await _fcmSender.SendMulticastAsync(title, body, data, tokens, context.CancellationToken);

            // 8) Ghi log từng device
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

                // token invalid -> xóa device
                if (!r.Success && r.Error is string)
                {
                    var err = r.Error as string;
                    if (err != null && err.Contains("registration-token-not-registered", StringComparison.OrdinalIgnoreCase))
                    {
                        var device = await _unitOfWork.DevicesRepository.GetByIdAsync(r.DeviceId);
                        if (device != null)
                        {
                            Console.WriteLine(r.DeviceId + " không hoạt động");
                            _unitOfWork.DevicesRepository.Remove(device);
                        }
                    }
                }
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}