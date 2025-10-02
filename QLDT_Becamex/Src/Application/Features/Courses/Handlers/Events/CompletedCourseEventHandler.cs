using MediatR;
using QLDT_Becamex.Src.Domain.Events;
using Quartz;
using static QLDT_Becamex.Src.Shared.Helpers.DateTimeHelper;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers.Events
{
    public sealed class CompletedCourseEventHandler : INotificationHandler<CompletedCourseEvent>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        
        public CompletedCourseEventHandler(ISchedulerFactory schedulerFactory) 
            => _schedulerFactory = schedulerFactory;

        public async Task Handle(CompletedCourseEvent e, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);

            var job = JobBuilder.Create<Infrastructure.Quartz.Jobs.CompletedCourseNotifyJob>()
                .WithIdentity($"CompletedCourseNotifyJob-{e.CourseId}")
                .UsingJobData("CourseId", e.CourseId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"CompletedCourseNotifyJob-{e.CourseId}")
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(3))
                .Build();

            await scheduler.ScheduleJob(job, trigger, ct);
        }
    }
}