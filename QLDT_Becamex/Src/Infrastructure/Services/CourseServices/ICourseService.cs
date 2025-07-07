using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Services.CourseServices
{
    public interface ICourseService
    {
        void UpdateCourseStatus(Course course);
    }
}
