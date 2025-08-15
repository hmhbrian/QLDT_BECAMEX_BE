using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Services.CourseServices
{
    public interface ICourseService
    {
        void UpdateCourseStatus(Course course);

        /// <summary>
        /// Tính và LƯU tiến độ + cập nhật trạng thái UserCourse vào DB.
        /// </summary>
        Task CalculateAndPersistAsync(string courseId, string userId, CancellationToken ct = default);

    }
}
