using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Persistence;
using QLDT_Becamex.Src.Infrastructure.Services.CourseServices;

namespace QLDT_Becamex.Src.Infrastructure.Services.BackgroundServices
{
    public class CourseStatusUpdateBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CourseStatusUpdateBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1); // Cập nhật mỗi ngày
        public CourseStatusUpdateBackgroundService(IServiceProvider serviceProvider, ILogger<CourseStatusUpdateBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Course Status Update Background Service is starting.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Updating course statuses at: {Time}", DateTime.Now);
                    await UpdateCourseStatusesAsync();
                }catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating course statuses.");
                }

                await Task.Delay(_interval, stoppingToken); // mỗi ngày cập nhật một lần
            }
            _logger.LogInformation("Course Status Update Background Service is stopping.");
        }
        private async Task UpdateCourseStatusesAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var courseService = scope.ServiceProvider.GetRequiredService<ICourseService>();

                var currentDate = DateTime.UtcNow;
                var courses = await unitOfWork.CourseRepository.GetCoursesForStatusUpdateAsync(currentDate);

                foreach (var course in courses)
                {
                    var originalStatus = course.StatusId;
                    courseService.UpdateCourseStatus(course);
                    if (course.StatusId != originalStatus)
                    {
                        unitOfWork.CourseRepository.UpdateEntity(course); // Cập nhật trạng thái khóa học
                    }
                }
                await unitOfWork.CompleteAsync(); // Lưu các thay đổi vào cơ sở dữ liệu
                _logger.LogInformation("Course statuses updated successfully.");
            }
        }
    }
}
