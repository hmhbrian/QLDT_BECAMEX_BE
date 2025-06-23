using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserStatusService
    {
        public Task<Result<IEnumerable<UserStatusDto>>> GetAllAsync();
        public Task<Result<UserStatusDto>> CreateAsync(UserStatusDtoRq rq);
        public Task<ApiResponse> UpdateAsync(int id, UserStatusDtoRq rq);
        public Task<ApiResponse> DeleteAsync(List<int> ids);
    }
}
