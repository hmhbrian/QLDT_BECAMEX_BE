using QLDT_Becamex.Src.Dtos.Params;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.Users;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        public Task<Result> RegisterAsync(RegisterDto registerDto);
        public Task<Result> SoftDeleteUserAsync(string userId);
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
        public Task<Result<PagedResult<UserDto>>> GetUsersAsync(BaseQueryParam queryParams);
    }
}
