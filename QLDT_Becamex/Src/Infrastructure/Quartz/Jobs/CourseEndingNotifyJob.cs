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
            var today = DateTime.Today;

            // 1) Lấy tất cả course sắp kết thúc
            var courses = await _unitOfWork.CourseRepository.GetFlexibleAsync(
                predicate: c => c.EndDate != null
            );

            foreach (var course in courses)
            {
                var daysUntilEnd = (course.EndDate - today)?.TotalDays;

                if (daysUntilEnd == 1 || daysUntilEnd == 2)
                {
                    // Lấy user đã đăng ký
                    var registeredUserCourses = await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                        predicate: uc => uc.CourseId == course.Id
                    );
                    if (!registeredUserCourses.Any()) continue;

                    var userIds = registeredUserCourses.Select(uc => uc.UserId).Distinct().ToList();

                    // Lấy devices
                    var devices = await _unitOfWork.DevicesRepository.GetFlexibleAsync(
                        predicate: d => d.UserId != null && userIds.Contains(d.UserId)
                    );
                    if (!devices.Any()) continue;

                    var tokens = devices
                        .Where(d => d.DeviceToken != null)
                        .Select(d => (DeviceId: d.Id, Token: d.DeviceToken!))
                        .ToList();

                    var (title, body, data) = await _notificationComposer.CourseEndingSoonAsync(course.Id, context.CancellationToken);
                    var now = ToVietnamTime(DateTime.UtcNow);
                    var dataJson = JsonSerializer.Serialize(data);

                    // Lưu message
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

                    // UserNotification
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

                    // Gửi FCM
                    var perTokenResults = await _fcmSender.SendMulticastAsync(title, body, data, tokens, context.CancellationToken);

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
