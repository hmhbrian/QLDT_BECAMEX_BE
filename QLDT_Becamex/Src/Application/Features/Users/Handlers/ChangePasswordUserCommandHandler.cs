using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;

public class ChangePasswordUserCommandHandler : IRequestHandler<ChangePasswordUserCommand, string>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserService _userService;

    public ChangePasswordUserCommandHandler(UserManager<ApplicationUser> userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    public async Task<string> Handle(ChangePasswordUserCommand command, CancellationToken cancellationToken)
    {
        var rq = command.Request;

        var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
        
        if (userId == null)
            throw new AppException("Không tìm thấy người dùng", 404);
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            throw new AppException("Không tìm thấy người dùng", 404);
        var result = await _userManager.ChangePasswordAsync(user, rq.OldPassword, rq.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new AppException($"Đổi mật khẩu thất bại: {errors}", 400);
        }

        return user.Id;
    }
}
