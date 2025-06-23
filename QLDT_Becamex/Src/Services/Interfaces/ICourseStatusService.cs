using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseStatusService
    {
        Task<Result<IEnumerable<CourseStatusDto>>> GetAllAsync();
        Task<ApiResponse> CreateAsync(CourseStatusDtoRq rq);
        Task<ApiResponse> UpdateAsync(int id, CourseStatusDtoRq rq);
        Task<ApiResponse> DeleteAsync(List<int> ids);
    }
}
