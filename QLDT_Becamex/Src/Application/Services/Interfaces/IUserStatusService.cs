using QLDT_Becamex.Src.Application.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserStatusService
    {
        public Task<Result<IEnumerable<UserStatusDto>>> GetAllAsync();
        public Task<Result<UserStatusDto>> CreateAsync(UserStatusDtoRq rq);
        public Task<Result> UpdateAsync(int id, UserStatusDtoRq rq);
        public Task<Result> DeleteAsync(List<int> ids);
    }
}
