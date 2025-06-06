using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        public Task<Result> RegisterAsync(RegisterDto registerDto);
    }
}
