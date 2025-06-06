using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.UnitOfWork;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public Task<UserDto> LoginAsync(LoginDto loginDto)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto)
        {
            var allowedRoles = new[] { "ADMIN", "LANHDAO", "HR", "NHANVIEN", "HOCVIEN" };
            var role = registerDto.Role?.ToUpper();

            // 1. Validate role
            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
            {
                return Result.Failure(
            error: "Role không hợp lệ. Chỉ được phép: ADMIN, LANHDAO, HR, NHANVIEN, HOCVIEN.",
            message: "Đăng ký thất bại",
            code: "INVALID_ROLE",
            statusCode: 400
        );
            }

            // 2. Tạo user
            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                IdCard = registerDto.IdCard,
                PhoneNumber = registerDto.NumberPhone
            };

            var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var emailExistError = createUserResult.Errors.FirstOrDefault(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName");
                if (emailExistError != null)
                {
                    return Result.Failure(
                        error: emailExistError.Description,
                        code: "EMAIL_EXIST",
                        statusCode: 400
                    );
                }

                return Result.Failure(
                    errors: createUserResult.Errors.Select(e => e.Description),
                    code: "USER_CREATION_FAILED",
                    statusCode: 400
                );
            }

            try
            {
                // 3. Đảm bảo role tồn tại
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }

                // 4. Gán role
                var addRoleResult = await _userManager.AddToRoleAsync(user, role);
                if (!addRoleResult.Succeeded)
                {
                    // Nếu gán role thất bại → XÓA USER đã tạo trước đó
                    await _userManager.DeleteAsync(user);
                    return Result.Failure(
                    error: "Không thể tạo role, vui lòng thử lại.",
                    code: "ROLE_CREATION_FAILED",
                    statusCode: 400
                );
                }

                return Result.Success(message: "Đăng ký thành công.", code: "REGISTER_SUCCESS");
            }
            catch (Exception ex)
            {
                // Nếu có lỗi bất ngờ → Xóa user và trả về lỗi
                await _userManager.DeleteAsync(user);
                return Result.Failure(
             error: ex.Message,
             code: "SYSTEM_ERROR",
             statusCode: 500
         );
            }
        }

    }
}
