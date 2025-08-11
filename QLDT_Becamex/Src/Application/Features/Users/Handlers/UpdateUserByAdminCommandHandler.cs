using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class UpdateUserByAdminCommandHandler : IRequestHandler<UpdateUserByAdminCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;

        public UpdateUserByAdminCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
             IUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public async Task<string> Handle(UpdateUserByAdminCommand command, CancellationToken cancellationToken)
        {
            var rq = command.Request;
            var userId = command.UserId;

            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();
            if (currentUserId == null)
            {
                throw new AppException("Không có quyền cập nhật thông tin người dùng", 403);

            }

            var userToUpdate = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (userToUpdate == null)
                throw new AppException("Không tìm thấy người dùng", 404);

            // Các đoạn logic cập nhật y như bạn viết ở service:
            if (!string.IsNullOrWhiteSpace(rq.FullName))
            {
                userToUpdate.FullName = rq.FullName;
                userToUpdate.NormalizedFullName = StringHelper.RemoveDiacritics(rq.FullName).ToUpperInvariant().Replace(" ", "");
            }

            if (!string.IsNullOrWhiteSpace(rq.NumberPhone))
                userToUpdate.PhoneNumber = rq.NumberPhone;

            if (rq.StartWork.HasValue)
                userToUpdate.StartWork = rq.StartWork;

            if (rq.EndWork.HasValue)
                userToUpdate.EndWork = rq.EndWork;

            if (rq.StatusId.HasValue)
                userToUpdate.StatusId = rq.StatusId;

            if (rq.DepartmentId.HasValue)
                userToUpdate.DepartmentId = rq.DepartmentId;

            if (!string.IsNullOrEmpty(rq.ManagerUId))
                userToUpdate.ManagerUId = rq.ManagerUId;

            if (rq.ELevelId.HasValue)
                userToUpdate.ELevelId = rq.ELevelId;

            if(rq.Position != null)
                userToUpdate.Position = rq.Position;

            userToUpdate.UpdateById = currentUserId;
            userToUpdate.ModifiedAt = DateTime.UtcNow;

            // Kiểm tra email có trùng
            if (!string.IsNullOrEmpty(rq.Email) && !string.Equals(userToUpdate.Email, rq.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _userManager.Users.AnyAsync(u => u.Id != userId && u.Email == rq.Email, cancellationToken);
                if (emailExists)
                    throw new AppException("Email đã được sử dụng", 409);

                await _userManager.SetEmailAsync(userToUpdate, rq.Email);
                await _userManager.SetUserNameAsync(userToUpdate, rq.Email.ToLowerInvariant());
            }

            // Kiểm tra CCCD
            if (!string.IsNullOrWhiteSpace(rq.IdCard) && !string.Equals(userToUpdate.IdCard, rq.IdCard, StringComparison.OrdinalIgnoreCase))
            {
                var idCardExists = await _userManager.Users.AnyAsync(u => u.Id != userId && u.IdCard == rq.IdCard, cancellationToken);
                if (idCardExists)
                    throw new AppException("CCCD đã tồn tại", 409);

                userToUpdate.IdCard = rq.IdCard;
            }

            // Kiểm tra mã nhân viên
            if (!string.IsNullOrWhiteSpace(rq.Code) && !string.Equals(userToUpdate.Code, rq.Code, StringComparison.OrdinalIgnoreCase))
            {
                var codeExists = await _userManager.Users.AnyAsync(u => u.Id != userId && u.Code == rq.Code, cancellationToken);
                if (codeExists)
                    throw new AppException("Mã nhân viên đã tồn tại", 409);

                userToUpdate.Code = rq.Code;
            }

            // Update role
            if (!string.IsNullOrEmpty(rq.RoleId))
            {
                var role = await _roleManager.FindByIdAsync(rq.RoleId);
                if (role == null || string.IsNullOrEmpty(role.Name))
                    throw new AppException("Vai trò không hợp lệ", 400);

                var currentRoles = await _userManager.GetRolesAsync(userToUpdate);
                if (!currentRoles.Contains(role.Name))
                {
                    await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
                    await _userManager.AddToRoleAsync(userToUpdate, role.Name);
                }
            }

            // Cập nhật cuối
            var updateResult = await _userManager.UpdateAsync(userToUpdate);
            if (!updateResult.Succeeded)
                throw new AppException("Lưu thông tin thất bại", 500);

            return userId;
        }
    }
}
