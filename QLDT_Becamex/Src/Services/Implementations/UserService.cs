using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Users;
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
            try
            {
                // 1. Tìm Position dựa trên PositionId
                // Không eager load Role ở đây
                Position? position = await _unitOfWork.PositionRepostiory.GetFirstOrDefaultAsync(
                    p => p.PositionId == registerDto.PositionId);

                if (position == null)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "ID vị trí không hợp lệ hoặc không tồn tại.",
                        code: "INVALID_POSITION_ID",
                        statusCode: 400
                    );
                }

                // 2. Lấy RoleName từ Position bằng cách truy vấn RoleManager
                if (string.IsNullOrEmpty(position.RoleId))
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Vị trí không được liên kết với một vai trò.",
                        code: "POSITION_MISSING_ROLE_ID",
                        statusCode: 400
                    );
                }

                // Tìm IdentityRole bằng RoleId
                var role = await _roleManager.FindByIdAsync(position.RoleId);
                if (role == null || string.IsNullOrEmpty(role.Name))
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Vai trò liên kết với vị trí không tồn tại hoặc không hợp lệ.",
                        code: "INVALID_POSITION_ROLE",
                        statusCode: 400
                    );
                }
                var roleNameFromPosition = role.Name; // Lấy tên vai trò từ đối tượng IdentityRole

                // 3. Tạo user
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
                    Code = registerDto.Code,
                    PositionId = registerDto.PositionId, // Gán PositionId từ DTO
                    // DepartmentId và ManagerId (nếu có trong RegisterDto, cần thêm vào)
                    // DepartmentId = registerDto.DepartmentId,
                    // ManagerId = registerDto.ManagerId,
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

                // 4. Đảm bảo role tồn tại (tên role lấy từ Position)
                if (!await _roleManager.RoleExistsAsync(roleNameFromPosition))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleNameFromPosition));
                }

                // 5. Gán role cho user
                var addRoleResult = await _userManager.AddToRoleAsync(user, roleNameFromPosition);
                if (!addRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Không thể gán vai trò cho người dùng, vui lòng thử lại.",
                        code: "ROLE_ASSIGNMENT_FAILED",
                        statusCode: 400
                    );
                }

                return Result.Success(message: "Đăng ký thành công.", code: "REGISTER_SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }


        public async Task<Result> SoftDeleteUserAsync(string userId)
        {
            // 1. Tìm người dùng theo ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure(
                    message: "Xóa người dùng thất bại",
                    error: "Người dùng không tồn tại.",
                    code: "USER_NOT_FOUND",
                    statusCode: 404 // Not Found
                );
            }

            // 2. Kiểm tra nếu người dùng đã bị xóa mềm rồi
            if (user.IsDeleted)
            {
                return Result.Failure(
                    message: "Xóa người dùng thất bại",
                    error: "Người dùng này đã bị xóa rồi.",
                    code: "USER_ALREADY_DELETED",
                    statusCode: 400 // Bad Request hoặc Conflict
                );
            }

            // 3. Thực hiện xóa mềm: Cập nhật IsDeleted thành true và đặt DeletedAt
            user.IsDeleted = true;
            user.ModifedAt = DateTime.UtcNow; // Cập nhật cả UpdatedAt

            // 4. Cập nhật người dùng trong UserManager
            // Lưu ý: UserManager.UpdateAsync sẽ lưu các thay đổi này vào DB
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Failure(
                    message: "Xóa người dùng thất bại",
                    errors: result.Errors.Select(e => e.Description),
                    code: "SOFT_DELETE_USER_FAILED", // Đổi mã lỗi để phản ánh xóa mềm
                    statusCode: 500 // Internal Server Error
                );
            }

            return Result.Success(
                message: "Xóa người dùng thành công (soft delete).", // Cập nhật thông báo
                code: "SOFT_DELETE_USER_SUCCESS",
                statusCode: 200
            );
        }
    }
}
