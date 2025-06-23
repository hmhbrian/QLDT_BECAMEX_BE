using QLDT_Becamex.Src.Application.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseService
    {
        Task<Result> CreateAsync(CourseDtoRq request);
        Task<Result> UpdateAsync(string id, CourseDtoRq request);
        Task<Result<CourseDto>> GetCourseAsync(string id);
        Task<Result<PagedResult<CourseDto>>> GetAllCoursesAsync(BaseQueryParam queryParam);
    }
}
