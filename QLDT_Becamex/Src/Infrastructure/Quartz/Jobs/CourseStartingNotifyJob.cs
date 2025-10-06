using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using Quartz;
using static QLDT_Becamex.Src.Shared.Helpers.DateTimeHelper;

namespace QLDT_Becamex.Src.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class CourseStartingNotifyJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFcmSender _fcmSender;
        private readonly INotificationComposer _notificationComposer;

        public CourseStartingNotifyJob(
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
            // Lấy ngày hiện tại theo timezone VN để so sánh ngày
            var today = ToVietnamTime(DateTime.UtcNow).Date;

            // 1) Lấy tất cả course có StartDate (có thể filter thêm nếu cần)
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: c => c.StartDate != null
            );

            foreach (var course in courses)
            {
                if (course.StartDate == null) continue;

                var daysUntilStart = (course.StartDate.Value.Date - today).TotalDays;

                // Chỉ gửi khi còn 1 hoặc 2 ngày nữa
                if (daysUntilStart == 1 || daysUntilStart == 2)
                {
                    // 2) Lấy danh sách UserId đã đăng ký khóa học
                    var registeredUserCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                        predicate: uc => uc.CourseId == course.Id
                    );

                    if (!registeredUserCourses.Any()) continue;

                    var userIds = registeredUserCourses.Select(uc => uc.UserId).Distinct().ToList();

                    // 3) Lấy tất cả devices của những user đã đăng ký
                    var devices = await _unitOfWork.DevicesRepository.GetFlexibleAsync(
                        predicate: d => d.UserId != null && userIds.Contains(d.UserId)
                    );

                    if (!devices.Any()) continue;

                    // 4) Tạo danh sách tokens để gửi FCM
                    var tokens = devices
                        .Where(d => d.DeviceToken != null)
                        .Select(d => (DeviceId: d.Id, Token: d.DeviceToken!))
                        .ToList();

                    // 5) Soạn payload
                    var (title, body, data) = await _notificationComposer.CourseStartingSoonAsync(course.Id, context.CancellationToken);

                    var now = ToVietnamTime(DateTime.UtcNow);
                    Console.WriteLine($"[Send] CourseStartingNotifyJob for course {course.Id} to {tokens.Count} devices at {now}");
                    var dataJson = JsonSerializer.Serialize(data);

                    // 6) Tạo message
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

                    // 7) Tạo UserNotifications cho từng user đã đăng ký
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

                    // 8) Gửi FCM theo batch (IFcmSender đã tự chunk 500 token/lần)
                    var perTokenResults = await _fcmSender.SendMulticastAsync(title, body, data, tokens, context.CancellationToken);

                    // 9) Ghi log từng device và remove device ko hợp lệ
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

                        if (!r.Success && r.Error is string err
                            && err.Contains("registration-token-not-registered", StringComparison.OrdinalIgnoreCase))
                        {
                            var device = await _unitOfWork.DevicesRepository.GetByIdAsync(r.DeviceId);
                            if (device != null)
                            {
                                _unitOfWork.DevicesRepository.Remove(device);
                            }
                        }
                    }

                    await _unitOfWork.CompleteAsync();
                }
            }
        }
    }
}