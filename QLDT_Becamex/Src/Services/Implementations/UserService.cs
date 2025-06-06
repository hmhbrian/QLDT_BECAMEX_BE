using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.UnitOfWork;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;

        public UserService(
            SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         IMapper mapper,
         IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {
            // 1. Tìm người dùng bằng tên đăng nhập hoặc email
            // Lý tưởng là bạn muốn tìm kiếm theo email nếu loginDto.Email được dùng cho email
            // hoặc bạn có một trường chung là "Identifier" cho cả email/username.
            // Hiện tại, bạn đang dùng loginDto.Email để tìm bằng UserName, điều này có thể gây nhầm lẫn.
            // Hãy đảm bảo rằng LoginDto.Email thực sự chứa email, và bạn tìm theo email.
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            // Nếu không tìm thấy bằng email, bạn có thể cân nhắc tìm bằng username nếu muốn linh hoạt

            if (user == null)
            {
                // Trả về lỗi chung để tránh lộ thông tin liệu email/username có tồn tại hay không.
                return Result<UserDto>.Failure(
                      message: "Đăng nhập thất bại",
                    error: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                    code: "INCORRECT_CREDENTIALS",
                    statusCode: 401 // Unauthorized
                );
            }

            // 2. Xác thực mật khẩu
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Đăng nhập thành công
                // Tạo UserDto
                var roles = await _userManager.GetRolesAsync(user);
                UserDto userDto = _mapper.Map<UserDto>(user);
                userDto.Role = roles.FirstOrDefault();

                // Trả về Result.Success với UserDto
                return Result<UserDto>.Success(
                    message: "Đăng nhập thành công.",
                    code: "SUCCESS",
                    statusCode: 200,
                    data: userDto
                );
            }
            else if (result.IsLockedOut)
            {
                // Tài khoản bị khóa
                return Result<UserDto>.Failure(
                      message: "Đăng nhập thất bại",
                    error: "Tài khoản của bạn đã bị khóa.",
                    code: "ACCOUNT_LOCKED_OUT",
                    statusCode: 403 // Forbidden
                );
            }
            else if (result.IsNotAllowed)
            {
                // Đăng nhập không được phép (ví dụ: email chưa xác nhận)
                return Result<UserDto>.Failure(
                      message: "Đăng nhập thất bại",
                    error: "Đăng nhập không được phép. Tài khoản của bạn chưa được xác nhận hoặc có vấn đề.",
                    code: "ACCOUNT_NOT_ALLOWED",
                    statusCode: 403 // Forbidden hoặc 400 Bad Request, tùy ngữ cảnh
                );
            }
            else
            {
                // Các trường hợp thất bại khác (ví dụ: sai mật khẩu)
                return Result<UserDto>.Failure(
                      message: "Đăng nhập thất bại",
                    error: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                    code: "INCORRECT_CREDENTIALS",
                    statusCode: 401 // Unauthorized
                );
            }
        }

        public async Task<Result> RegisterAsync(RegisterDto registerDto)
        {
            var allowedRoles = new[] { "ADMIN", "LANHDAO", "HR", "NHANVIEN", "HOCVIEN" };
            var role = registerDto.Role?.ToUpper();

            // 1. Validate role
            if (string.IsNullOrEmpty(role) || !allowedRoles.Contains(role))
            {
                return Result.Failure(
            message: "Đăng ký thất bại",
            error: "Role không hợp lệ. Chỉ được phép: ADMIN, LANHDAO, HR, NHANVIEN, HOCVIEN.",
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
                PhoneNumber = registerDto.NumberPhone,
                StartWork = registerDto.StartWork,
                EndWork = registerDto.EndWork,
                CreatedAt = DateTime.UtcNow,
            };

            var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);
            if (!createUserResult.Succeeded)
            {
                var emailExistError = createUserResult.Errors.FirstOrDefault(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName");
                if (emailExistError != null)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: emailExistError.Description,
                        code: "EMAIL_EXIST",
                        statusCode: 400
                    );
                }

                return Result.Failure(
                    message: "Đăng ký thất bại",
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
                    message: "Đăng ký thất bại",
                    error: "Không thể tạo role, vui lòng thử lại.",
                    code: "ROLE_CREATION_FAILED",
                    statusCode: 400
                );
                }

                return Result.Success(message: "Đăng ký thành công.", code: "REGISTER_SUCCESS", statusCode: 200);
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
