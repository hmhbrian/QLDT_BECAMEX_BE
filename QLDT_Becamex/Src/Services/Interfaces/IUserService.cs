using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Users;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<Result<UserDto>> LoginAsync(LoginDto loginDto);
        public Task<Result> RegisterAsync(RegisterDto registerDto);
    }
}
