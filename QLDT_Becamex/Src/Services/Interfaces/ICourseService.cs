using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Results;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Result> CreateAsync(CourseDtoRq request);
        Task<Result> UpdateAsync(string id, CourseDtoRq request);
        Task<Result<CourseDto>> GetCourseAsync(string id);
    }
}
