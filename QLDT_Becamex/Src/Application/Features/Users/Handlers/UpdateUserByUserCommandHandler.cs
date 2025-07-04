// File: QLDT_Becamex.Src/Application/Commands/Users/CreateUser/CreateUserCommandHandler.cs
// This handler contains the core business logic for creating a user.
// It interacts with Identity services and returns a FluentResults.Result object.

using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Domain.Entities;
using FluentResults; // FluentResults for rich result handling
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Infrastructure.Services.CloudinaryServices;

namespace QLDT_Becamex.Src.Application.Commands.Users.CreateUser
{
    // Handler for CreateUserCommand.
    // It implements IRequestHandler to process the command and return a Result<string> (user ID).
    public class UpdateUserByUserCommandHandler : IRequestHandler<UpdateUserByUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserService _userService;

        public UpdateUserByUserCommandHandler(
            UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService, IUserService userService)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _userService = userService;
        }

        public async Task<string> Handle(UpdateUserByUserCommand command, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var request = command.Request;
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            // 1. Tìm người dùng cần cập nhật
            var userToUpdate = await _userManager.FindByIdAsync(userId!);

            if (userToUpdate == null)
            {
                throw new AppException("Unauthorized", 403);
            }

            string? imageUrl = null;
            if (request.UrlAvatar != null)
            {
                imageUrl = await _cloudinaryService.UploadImageAsync(request.UrlAvatar);
            }

            // Cập nhật các trường nếu có dữ liệu mới
            if (!string.IsNullOrWhiteSpace(request.FullName))
                userToUpdate.FullName = request.FullName;

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                userToUpdate.PhoneNumber = request.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(imageUrl))
                userToUpdate.UrlAvatar = imageUrl;

            // Cần gọi UserManager.UpdateAsync để lưu thay đổi vào database
            var updateResult = await _userManager.UpdateAsync(userToUpdate);
            if (!updateResult.Succeeded)
            {
                throw new AppException("Cập nhật thông tin cá nhân thất bại", 400);
            }

            return userToUpdate.Id;
        }
    }
}