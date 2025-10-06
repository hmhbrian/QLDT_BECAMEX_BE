using QLDT_Becamex.Src.Infrastructure.Quartz.Jobs;
using Quartz;
using TimeZoneConverter;

namespace QLDT_Becamex.Src.Infrastructure.Quartz
{
    public static class QuartzRegistrar
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                //Đăng ký ReviewReminderJob
                var tz = TZConvert.GetTimeZoneInfo("Asia/Ho_Chi_Minh");

                var reviewJobKey = new JobKey("ReviewReminderJob");
                q.AddJob<ReviewReminderJob>(opts => opts.WithIdentity(reviewJobKey));

                q.AddTrigger(t => t
                    .ForJob(reviewJobKey)
                    .WithIdentity("ReviewReminderJob-Trigger")
                    .WithCronSchedule("0 0 8 * * ?", x => x.InTimeZone(tz)) // 8h sáng hằng ngày
                );

                var startingJobKey = new JobKey("CourseStartingNotifyJob");
                q.AddJob<CourseStartingNotifyJob>(opts => opts.WithIdentity(startingJobKey));

                q.AddTrigger(t => t
                    .ForJob(startingJobKey)
                    .WithIdentity("CourseStartingNotifyJob-Trigger")
                    .WithCronSchedule("0 19 10 * * ?", x => x.InTimeZone(tz))
                );

                var endingJobKey = new JobKey("CourseEndingNotifyJob");
                q.AddJob<CourseEndingNotifyJob>(opts => opts.WithIdentity(endingJobKey));
                
                q.AddTrigger(t => t
                    .ForJob(endingJobKey)
                    .WithIdentity("CourseEndingNotifyJob-Trigger")
                    .WithCronSchedule("0 0 9 * * ?", x => x.InTimeZone(tz)) // 8h sáng hằng ngày
                );
                // Nếu có job khác, add tiếp ở đây
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            return services;
        }
    }
}
