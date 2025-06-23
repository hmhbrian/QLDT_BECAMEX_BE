using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(UserLoginRq loginDto);
        public Task<ApiResponse> CreateUserAsync(UserDtoRq rq);
        public Task<ApiResponse> SoftDeleteUserAsync(string userId);
        public Task<Result<UserDto>> GetUserAsync(string userId);
        public Task<Result<PagedResult<UserDto>>> GetUsersAsync(BaseQueryParam queryParams);
        public Task<Result<PagedResult<UserDto>>> SearchUserAsync(string keyword, BaseQueryParam queryParams);
        public Task<ApiResponse> UpdateMyProfileAsync(string userId, UserUpdateSelfDtoRq rq);
        public Task<ApiResponse> UpdateUserByAdmin(string userId, AdminUpdateUserDtoRq rq);
        public Task<ApiResponse> ChangePasswordUserAsync(string userId, UserChangePasswordRq rq);
        public Task<ApiResponse> ResetPasswordByAdminAsync(string userId, UserResetPasswordRq rq);
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
    }
}
