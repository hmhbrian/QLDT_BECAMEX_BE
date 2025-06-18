using QLDT_Becamex.Src.Dtos.Params;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Dtos.Users;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(UserLoginRq loginDto);
        public Task<Result> CreateUserAsync(UserDtoRq rq);
        public Task<Result> SoftDeleteUserAsync(string userId);
        public Task<Result<UserDto>> GetUserAsync(string userId);
        public Task<Result<PagedResult<UserDto>>> GetUsersAsync(BaseQueryParam queryParams);
        public Task<Result<PagedResult<UserDto>>> SearchUserAsync(string keyword, BaseQueryParam queryParams);
        public Task<Result> UpdateMyProfileAsync(string userId, UserUpdateSelfDtoRq rq);
        public Task<Result> UpdateUserByAdmin(string userId, AdminUpdateUserDtoRq rq);

        public Task<Result> ChangePasswordUserAsync(string userId, UserChangePasswordRq rq);
        public Task<Result> ResetPasswordByAdminAsync(string userId, UserResetPasswordRq rq);

        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
    }
}
