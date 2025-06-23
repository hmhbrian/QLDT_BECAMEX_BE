using QLDT_Becamex.Src.Application.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseStatusService
    {
        Task<Result<IEnumerable<CourseStatusDto>>> GetAllAsync();
        Task<Result> CreateAsync(CourseStatusDtoRq rq);
        Task<Result> UpdateAsync(int id, CourseStatusDtoRq rq);
        Task<Result> DeleteAsync(List<int> ids);
    }
}
