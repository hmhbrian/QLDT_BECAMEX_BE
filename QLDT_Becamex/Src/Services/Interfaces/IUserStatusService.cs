using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.UserStatus;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserStatusService
    {
        Task<Result<IEnumerable<UserStatusDto>>> GetAllAsync();
        Task<Result<UserStatusDto>> CreateAsync(UserStatusDtoRq rq);
        Task<Result> UpdateAsync(int id, UserStatusDtoRq rq);
        Task<Result> DeleteAsync(int id);
    }
}
