using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(UserLoginDto loginDto);
        public Task<Result> CreateUserAsync(UserCreateDto rq);
        public Task<Result> SoftDeleteUserAsync(string userId);
        public Task<Result<UserDto>> GetUserAsync(string userId);
        public Task<Result<PagedResult<UserDto>>> GetUsersAsync(BaseQueryParam queryParams);
        public Task<Result<PagedResult<UserDto>>> SearchUserAsync(string keyword, BaseQueryParam queryParams);
        public Task<Result> UpdateMyProfileAsync(string userId, UserUserUpdateDto rq);
        public Task<Result> UpdateUserByAdmin(string userId, UserAdminUpdateDto rq);
        public Task<Result> ChangePasswordUserAsync(string userId, UserChangePasswordDto rq);
        public Task<Result> ResetPasswordByAdminAsync(string userId, UserResetPasswordDto rq);
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
    }
}
