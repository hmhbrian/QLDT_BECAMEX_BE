
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;
using AutoMapper;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Infrastructure.Services.JwtServices;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, UserDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;

        public LoginUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(LoginUserCommand command, CancellationToken cancellationToken)
        {
            var loginDto = command.Request;

            // 1. Tìm người dùng theo email
            var user = await _userManager.Users
                .Include(u => u.ELevel)
                .Include(u => u.Department)
                .Include(u => u.ManagerU)
                .Include(u => u.UserStatus)
                .Include(u => u.CreateBy)
                .Include(u => u.CreateBy)
                .Include(u => u.CreateBy)
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email, cancellationToken);

            if (user == null)
                throw new AppException("Tên đăng nhập hoặc mật khẩu không chính xác.", 401);

            // 2. Kiểm tra mật khẩu
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (user.StatusId == 2)
                {
                    throw new AppException("Bạn đã nghĩ việc, không thể đăng nhập.", 401);
                }
                var userDto = _mapper.Map<UserDto>(user);
                var roles = await _userManager.GetRolesAsync(user);
                userDto.Role = roles.FirstOrDefault();
                userDto.AccessToken = _jwtService.GenerateJwtToken(user.Id, user.Email!, userDto.Role!);

                return userDto;
            }

            if (result.IsLockedOut)
                throw new AppException("Tài khoản của bạn đã bị khóa.", 403);

            if (result.IsNotAllowed)
                throw new AppException("Tài khoản chưa được xác nhận hoặc bị hạn chế.", 403);

            // Các trường hợp thất bại còn lại
            throw new AppException("Tên đăng nhập hoặc mật khẩu không chính xác.", 401);
        }
    }
}
