using MediatR;
using QLDT_Becamex.Src.Domain.Events;
using Quartz;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers.Events
{
    public sealed class CourseCreatedEventHandler : INotificationHandler<CourseCreatedEvent>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public CourseCreatedEventHandler(ISchedulerFactory schedulerFactory) => _schedulerFactory = schedulerFactory;

        public async Task Handle(CourseCreatedEvent e, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);

            var job = JobBuilder.Create<Infrastructure.Quartz.Jobs.CourseCreatedNotifyJob>()
                .WithIdentity($"CourseCreatedNotifyJob-{e.CourseId}")
                .UsingJobData("CourseId", e.CourseId.ToString())
                .UsingJobData("DepartmentIds", string.Join(",", e.DepartmentIds ?? Array.Empty<string>()))
                .UsingJobData("Levels", string.Join(",", e.Levels ?? Array.Empty<string>()))
                .UsingJobData("MandatoryUserIds", string.Join(",", e.MandatoryUserIds ?? Array.Empty<string>()))
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"CourseCreatedNotifyTrigger-{e.CourseId}")
                .StartAt(DateTimeOffset.UtcNow.AddSeconds(3))
                .Build();

            await scheduler.ScheduleJob(job, trigger, ct);
        }
    }
}
