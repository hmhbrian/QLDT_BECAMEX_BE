using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLDT_Becamex.Src.Application.Features.Notifications.Abstractions;
using QLDT_Becamex.Src.Application.Features.Notifications.Services;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Fcm;
using QLDT_Becamex.Src.Infrastructure.Services.NotificationService;
using Quartz;

namespace QLDT_Becamex.Src.Infrastructure.Quartz.Jobs
{
    [DisallowConcurrentExecution]
    public sealed class ReviewReminderJob : IJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReviewReminderComposer _composer;
        private readonly INotificationService _service;
        private readonly IRecipientResolver _resolver;
        private readonly ILogger<ReviewReminderJob> _logger;

        public ReviewReminderJob(
            IUnitOfWork unitOfWork,
            INotificationService service, 
            IReviewReminderComposer composer, 
            IRecipientResolver resolver,
            ILogger<ReviewReminderJob> logger)
        {
            _unitOfWork = unitOfWork;
            _service = service;
            _composer = composer;
            _resolver = resolver;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;

            // 1) Lấy tất cả userCourse hoàn thành
            var completedCourses = (await _unitOfWork.UserCourseRepository.GetFlexibleAsync(
                predicate: u => u.Status == 3,
                asNoTracking: true,
                includes: uc => uc.Include(u => u.User)
                                    .Include(c => c.Course)
            )).ToList();
            _logger.LogInformation("ReviewReminderJob: completed={n}", completedCourses.Count);


            // 2) Lọc ra những userCourse chưa có feedback
            var reminders = new List<(ApplicationUser User, Course Course)>();
            foreach (var uc in completedCourses)
            {
                bool hasFeedback = await _unitOfWork.FeedbackRepository.AnyAsync(
                    f => f.UserId == uc.UserId && f.CourseId == uc.CourseId);

                if (!hasFeedback)
                {
                    if (uc.User != null && uc.Course != null)
                    {
                        reminders.Add((uc.User, uc.Course));
                    }
                }
            }
            _logger.LogInformation("ReviewReminderJob: pending(no feedback)={n}", reminders.Count);

            if (reminders.Count == 0) return;

            // 3) Gửi thông báo
            foreach (var (user, course) in reminders)
            {
                // Lấy device tokens cho user
                var tokens = await _resolver.ResolveByUserIdsAsync(new[] { user.Id }, ct);

                _logger.LogInformation("User {UserId} has {Count} tokens", user.Id, tokens.Count);
                foreach (var t in tokens)
                    _logger.LogInformation("Device {Id}: {Token}", t.DeviceId, t.Token);

                if (tokens.Count == 0) continue;

                // Soạn thông báo cá nhân hóa
                var (title, body, data) = _composer.Build(
                    user.FullName ?? "Bạn",
                    course.Name ?? "khóa học",
                    course.Id);
                //Lưu message, log và gửi thông báo
                await _service.SendNotificationAsync(title, body, data, tokens, context.CancellationToken);


            }
        }
    }
}
