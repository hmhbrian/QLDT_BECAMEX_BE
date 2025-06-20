using QLDT_Becamex.Src.Dtos.Courses;
using QLDT_Becamex.Src.Dtos.Results;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseStatusService
    {
        Task<Result<IEnumerable<CourseSatusDto>>> GetAllAsync();
        Task<Result> CreateAsync(CourseStatusDtoRq rq);
        Task<Result> UpdateAsync(int id, CourseStatusDtoRq rq);
        Task<Result> DeleteAsync(List<int> ids);
    }
}
