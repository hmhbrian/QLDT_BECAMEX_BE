using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class ForceDeleteUserCommandHandler : IRequestHandler<ForceDeleteUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForceDeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(ForceDeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new AppException("Người dùng không tồn tại.", 404);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                throw new AppException($"Xóa vĩnh viễn thất bại: {errors}", 400);
            }

            return user.Id;
        }
    }
}
