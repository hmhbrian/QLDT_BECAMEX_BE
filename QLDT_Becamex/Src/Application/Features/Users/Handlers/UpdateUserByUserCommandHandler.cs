// File: QLDT_Becamex.Src/Application/Commands/Users/UpdateUser/UpdateUserCommandHandler.cs
// This handler updates a user's profile information.

using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Commands.Users.UpdateUser
{
    // Handler for updating user by themselves (profile update)
    public class UpdateUserByUserCommandHandler : IRequestHandler<UpdateUserByUserCommand, UserUserUpdateDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserService _userService;

        public UpdateUserByUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICloudinaryService cloudinaryService,
            IUserService userService)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _userService = userService;
        }

        public async Task<UserUserUpdateDto> Handle(UpdateUserByUserCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var request = command.Request;

            // Lấy ID người dùng hiện tại từ token
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();

            var userToUpdate = await _userManager.FindByIdAsync(userId!);
            if (userToUpdate == null)
                throw new AppException("Unauthorized", 403);

            string? imageUrl = null;
            if (request.UrlAvatar != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(request.UrlAvatar);
            }

            // Cập nhật nếu có giá trị mới và khác giá trị hiện tại
            if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != userToUpdate.FullName)
            {
                userToUpdate.FullName = request.FullName;
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber != userToUpdate.PhoneNumber)
            {
                userToUpdate.PhoneNumber = request.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(imageUrl))
                userToUpdate.UrlAvatar = imageUrl;

            var updateResult = await _userManager.UpdateAsync(userToUpdate);
            if (!updateResult.Succeeded)
                throw new AppException("Cập nhật thông tin cá nhân thất bại", 400);

            return new UserUserUpdateDto
            {
                FullName = userToUpdate.FullName,
                PhoneNumber = userToUpdate.PhoneNumber,
                UrlAvatar = userToUpdate.UrlAvatar
            };
        }
    }
}
