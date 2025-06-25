using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SoftDeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new AppException("Người dùng không tồn tại", 404);

            if (user.IsDeleted)
                throw new AppException("Người dùng này đã bị xóa rồi", 409);

            user.IsDeleted = true;
            user.ModifiedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new AppException("Xóa người dùng thất bại: " +
                    string.Join(" ", result.Errors.Select(e => e.Description)), 400);

            return user.Id;
        }
    }
}
