using MediatR;
using QLDT_Becamex.Src.Domain.Events;
using Quartz;
using static QLDT_Becamex.Src.Shared.Helpers.DateTimeHelper;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers.Events
{
    public sealed class CourseEndingEventHandler : INotificationHandler<CourseEndingEvent>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        
        public CourseEndingEventHandler(ISchedulerFactory schedulerFactory) 
            => _schedulerFactory = schedulerFactory;

        public async Task Handle(CourseEndingEvent e, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);

            // Trong CourseStartingEventHandler.cs
            if (!e.endDate.HasValue)
                return;
            var triggerTime = e.endDate.Value.AddDays(-2);
            // Nếu thời điểm trigger đã qua, không schedule
            if (triggerTime <= ToVietnamTime(DateTime.UtcNow))
                return;
            Console.WriteLine($"[Schedule] Course {e.CourseId} at {triggerTime}");
            var job = JobBuilder.Create<Infrastructure.Quartz.Jobs.CourseEndingNotifyJob>()
                .WithIdentity($"CourseEndingNotifyJob-{e.CourseId}")
                .UsingJobData("CourseId", e.CourseId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"CourseEndingNotifyTrigger-{e.CourseId}")
                .StartAt(new DateTimeOffset(triggerTime))
                .Build();

            await scheduler.ScheduleJob(job, trigger, ct);
        }
    }
}