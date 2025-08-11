// File: QLDT_Becamex.Src/Application/Commands/Users/CreateUser/CreateUserCommandHandler.cs
// This handler contains the core business logic for creating a user.
// It interacts with Identity services and returns a FluentResults.Result object.

using FluentResults; // FluentResults for rich result handling
using MediatR;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QLDT_Becamex.Src.Application.Commands.Users.CreateUser
{
    // Handler for CreateUserCommand.
    // It implements IRequestHandler to process the command and return a Result<string> (user ID).
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
             IUserService userService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {


            cancellationToken.ThrowIfCancellationRequested();


            var request = command.Request;

            var (currentUserId, _) = _userService.GetCurrentUserAuthenticationInfo();

            // 1. Kiểm tra role
            var role = await _roleManager.FindByNameAsync("HOCVIEN");
            if (role == null)
                throw new AppException("Role mặc định không tồn tại", 500);

            // 2. Kiểm tra trùng email
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
                throw new AppException("Email đã được sử dụng", 409);

            if(await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(c => c.PhoneNumber == request.NumberPhone) is not null)
                throw new AppException("Số điện thoại đã được sử dụng", 409);

            // 3. Tạo user
            var user = new ApplicationUser
            {
                UserName = request.Email.ToLower(),
                Email = request.Email.ToLower(),
                FullName = request.FullName,
                IdCard = request.IdCard,
                PhoneNumber = request.NumberPhone,
                StartWork = request.StartWork,
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
                Code = request.Code,
                Position = request.Position,
                DepartmentId = request.DepartmentId,
                ELevelId = request.ELevelId,
                StatusId = request.StatusId,
                ManagerUId = request.ManagerUId,
                CreateById = currentUserId,
                NormalizedFullName = StringHelper.RemoveDiacritics(request.FullName).ToUpperInvariant().Replace(" ", "")
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
                throw new AppException("Tạo user thất bại", 500);

            await _userManager.AddToRoleAsync(user, role.Name!);

            return user.Id;
        }
    }
}