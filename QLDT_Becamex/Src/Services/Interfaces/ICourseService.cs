using System.Collections.Generic;
using System.Threading.Tasks;
using QLDT_Becamex.Src.Dtos.Courses;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CourseDto dto);
        Task<bool> UpdateCourseAsync(int id, CourseDto dto);
        Task<bool> DeleteCourseAsync(int id);
    }
}
