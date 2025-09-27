using Quartz;

namespace QLDT_Becamex.Src.Infrastructure.Quartz
{
    public static class QuartzRegistrar
    {
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                // Với Quartz >= 3.3.x, job factory mặc định đã scoped (có thể không cần gọi UseMicrosoftDependencyInjectionJobFactory)
                // q.UseMicrosoftDependencyInjectionJobFactory(); // nếu muốn giữ như các ví dụ quen thuộc

                // (Tuỳ chọn) add job keys dùng lại; ở đây mình schedule động nên không cần add sẵn.
            });

            services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);

            return services;
        }
    }
}
