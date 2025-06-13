using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Dtos.Params;
using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Dtos.Results;
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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         IMapper mapper,
         IUnitOfWork unitOfWork,
         IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }




        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto)
        {

            // Hãy đảm bảo rằng LoginDto.Email thực sự chứa email, và bạn tìm theo email.
            var user = await _userManager.Users
                               .Include(u => u.Position)
                               .Include(u => u.Department)
                               .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
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
                PositionDto positionDto = _mapper.Map<PositionDto>(user.Position);
                var roles = await _userManager.GetRolesAsync(user);
                UserDto userDto = _mapper.Map<UserDto>(user);
                userDto.Role = roles.FirstOrDefault();
                userDto.Position = positionDto;
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
                string targetRoleId;    // ID của vai trò sẽ gán cho người dùng mới
                string targetRoleName;  // Tên của vai trò sẽ gán cho người dùng mới

                // 1. Xác định vai trò sẽ gán cho người dùng mới
                if (!string.IsNullOrEmpty(registerDto.RoleId))
                {
                    // Nếu RoleId được cung cấp trong DTO, sử dụng nó
                    var submittedRole = await _roleManager.FindByIdAsync(registerDto.RoleId);
                    if (submittedRole == null || string.IsNullOrEmpty(submittedRole.Name))
                    {
                        return Result.Failure(
                            message: "Đăng ký thất bại",
                            error: "ID vai trò được cung cấp không hợp lệ hoặc không tồn tại.",
                            code: "INVALID_ROLE_ID_SUBMITTED",
                            statusCode: 400
                        );
                    }
                    targetRoleId = submittedRole.Id;
                    targetRoleName = submittedRole.Name;
                }
                else
                {
                    // Nếu RoleId không được cung cấp, mặc định là "HOCVIEN"
                    var hocVienRole = await _roleManager.FindByNameAsync("HOCVIEN");
                    if (hocVienRole == null)
                    {
                        return Result.Failure(
                            message: "Đăng ký thất bại",
                            error: "Vai trò 'HOCVIEN' không tồn tại trong hệ thống. Vui lòng cấu hình vai trò này.",
                            code: "HOCVIEN_ROLE_NOT_FOUND",
                            statusCode: 500 // Lỗi cấu hình hệ thống
                        );
                    }
                    targetRoleId = hocVienRole.Id;
                    targetRoleName = hocVienRole.Name;
                }

                // 2. Xác thực Email đã tồn tại trước khi tạo user
                var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUserByEmail != null)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Email này đã được sử dụng bởi một tài khoản khác.",
                        code: "EMAIL_ALREADY_EXISTS",
                        statusCode: 400
                    );
                }

                // Tìm IdentityRole bằng RoleId
                var role = await _roleManager.FindByIdAsync(registerDto.RoleId);
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

                // 4. Tạo user
                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    IdCard = registerDto.IdCard,
                    PhoneNumber = registerDto.NumberPhone,
                    StartWork = registerDto.StartWork,
                    CreatedAt = DateTime.Now,
                    Code = registerDto.Code,
                    PositionId = registerDto.PositionId, // Sử dụng finalPositionId đã xác định
                    // DepartmentId và ManagerId (nếu có trong RegisterDto, cần thêm vào)
                };

                var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createUserResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        errors: createUserResult.Errors.Select(e => e.Description),
                        code: "USER_CREATION_FAILED",
                        statusCode: 400
                    );
                }

                // 5. Gán role cho user
                var addRoleResult = await _userManager.AddToRoleAsync(user, targetRoleName);
                if (!addRoleResult.Succeeded)
                {
                    // Nếu gán role thất bại -> XÓA USER đã tạo trước đó để tránh user không có role
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
                // Ghi log lỗi chi tiết tại đây (ví dụ: ILogger)
                // Console.WriteLine($"Error registering user: {ex.Message}");
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> SoftDeleteUserAsync(string userId)
        {
            try
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


        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;
            var userId = currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var role = currentUser?.FindFirst(ClaimTypes.Role)?.Value;

            return (userId, role);
        }


        public async Task<Result<PagedResult<UserDto>>> GetUsersAsync(BaseQueryParam queryParams) // Return type remains Result<PagedResult<UserDto>>
        {
            try
            {
                // 1. Get total item count (before pagination and based on filter)
                int totalItems = await _unitOfWork.UserRepository.CountAsync(u => !u.IsDeleted);

                // 2. Build the sorting function for GetAsync
                Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderByFunc = query =>
                {
                    bool isDesc = queryParams.SortType?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

                    return queryParams.SortField?.ToLower() switch
                    {
                        "email" => isDesc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                        "created.at" => isDesc ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                        _ => query.OrderBy(u => u.Email) // fallback
                    };
                };

                // 3. Call GetAsync from UserRepository to fetch user data
                var users = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: u => !u.IsDeleted,
                    orderBy: orderByFunc,
                    page: queryParams.Page,
                    pageSize: queryParams.Limit,
                    asNoTracking: true

                );

                // 4. Calculate pagination metadata
                int effectiveLimit = queryParams.Limit > 0 ? queryParams.Limit : 1;
                int totalPages = (int)Math.Ceiling((double)totalItems / effectiveLimit);
                var paginationInfo = new Pagination // Use Pagination as per your updated DTO
                {
                    TotalItems = totalItems,
                    ItemsPerPage = effectiveLimit,
                    CurrentPage = queryParams.Page,
                    TotalPages = totalPages
                };

                // 5. Map to UserDto and fetch roles
                var userDtos = _mapper.Map<List<UserDto>>(users);

                // 6. Create the PagedResult<UserDto> instance
                var pagedResultData = new PagedResult<UserDto> // Use 'new' as it's a plain class now
                {
                    Items = userDtos,
                    Pagination = paginationInfo
                };

                // 7. Wrap the PagedResult<UserDto> in your main Result<T> wrapper
                return Result<PagedResult<UserDto>>.Success(
                    pagedResultData,
                    message: "Successfully retrieved user list.",
                    code: "GET_ALL_USERS_SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Log the exception details
                return Result<PagedResult<UserDto>>.Failure(
                    error: ex.Message,
                    message: "An error occurred while retrieving the user list.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}
