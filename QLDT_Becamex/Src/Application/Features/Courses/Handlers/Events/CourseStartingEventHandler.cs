using MediatR;
using QLDT_Becamex.Src.Domain.Events;
using Quartz;
using static QLDT_Becamex.Src.Shared.Helpers.DateTimeHelper;

namespace QLDT_Becamex.Src.Application.Features.Courses.Handlers.Events
{
    public sealed class CourseStartingEventHandler : INotificationHandler<CourseStartingEvent>
    {
        private readonly ISchedulerFactory _schedulerFactory;
        
        public CourseStartingEventHandler(ISchedulerFactory schedulerFactory) 
            => _schedulerFactory = schedulerFactory;

        public async Task Handle(CourseStartingEvent e, CancellationToken ct)
        {
            var scheduler = await _schedulerFactory.GetScheduler(ct);

            // Tính thời điểm trigger: 2 ngày trước StartDate
            // Trong CourseStartingEventHandler.cs
            if (!e.startDate.HasValue)
                return;
            var triggerTime = e.startDate.Value.AddDays(-2);
            // Nếu thời điểm trigger đã qua, không schedule
            if (triggerTime <= ToVietnamTime(DateTime.UtcNow))
                return;
            Console.WriteLine($"[Schedule] Course {e.CourseId} at {triggerTime}");
            var job = JobBuilder.Create<Infrastructure.Quartz.Jobs.CourseStartingNotifyJob>()
                .WithIdentity($"CourseStartingNotifyJob-{e.CourseId}")
                .UsingJobData("CourseId", e.CourseId)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"CourseStartingNotifyTrigger-{e.CourseId}")
                .StartAt(new DateTimeOffset(triggerTime))
                .Build();

            await scheduler.ScheduleJob(job, trigger, ct);
        }
    }
}