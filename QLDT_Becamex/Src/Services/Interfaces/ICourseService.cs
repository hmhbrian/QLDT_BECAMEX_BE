using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface ICourseService
    {
        Task<ApiResponse> CreateAsync(CourseDtoRq request);
        Task<ApiResponse> UpdateAsync(string id, CourseDtoRq request);
    }
}
