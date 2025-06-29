using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class ResetPasswordByAdminCommandHandler : IRequestHandler<ResetPasswordByAdminCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ResetPasswordByAdminCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(ResetPasswordByAdminCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user == null)
            {
                throw new AppException("Không tìm thấy người dùng.", 404);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, command.Request.NewPassword);

            if (!resetResult.Succeeded)
            {
                var error = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                throw new AppException($"Đặt lại mật khẩu thất bại: {error}", 400);
            }

            return user.Id;
        }
    }
}
