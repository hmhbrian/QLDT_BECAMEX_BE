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
using System.IdentityModel.Tokens.Jwt;
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




        public async Task<Result<UserDto>> LoginAsync(UserLogin loginDto)
        {

            // Hãy đảm bảo rằng LoginDto.Email thực sự chứa email, và bạn tìm theo email.
            var user = await _userManager.Users
                               .Include(u => u.Position)
                               .Include(u => u.Department)
                               .Include(u => u.ManagerU)
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

        public async Task<Result> CreateUserAsync(UserDtoRq registerDto)
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
                var role = await _roleManager.FindByIdAsync(targetRoleId);
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
                    UserName = registerDto.Email.ToLower(),
                    Email = registerDto.Email.ToLower(),
                    FullName = registerDto.FullName,
                    IdCard = registerDto.IdCard,
                    PhoneNumber = registerDto.NumberPhone,
                    StartWork = registerDto.StartWork,
                    CreatedAt = DateTime.Now,
                    Code = registerDto.Code,
                    PositionId = registerDto.PositionId,
                    ManagerUId = registerDto.ManagerUId// Sử dụng finalPositionId đã xác định
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

        public async Task<Result> UpdateUserAsync(string userId, UserDtoRq updateDto)
        {
            try
            {
                // 1. Tìm người dùng cần cập nhật
                var userToUpdate = await _userManager.FindByIdAsync(userId);
                if (userToUpdate == null)
                {
                    return Result.Failure(
                        message: "Cập nhật người dùng thất bại",
                        error: $"Không tìm thấy người dùng với ID: {userId}.",
                        code: "USER_NOT_FOUND",
                        statusCode: 404
                    );
                }

                // 2. Cập nhật các trường thông tin cơ bản
                userToUpdate.FullName = updateDto.FullName; // Required, nên luôn có giá trị
                userToUpdate.IdCard = updateDto.IdCard;
                userToUpdate.Code = updateDto.Code;
                userToUpdate.PhoneNumber = updateDto.NumberPhone;
                userToUpdate.StartWork = updateDto.StartWork; // Có thể là null trong DTO
                userToUpdate.EndWork = updateDto.EndWork;     // Cần thêm vào UserDtoRq nếu muốn cập nhật
                userToUpdate.StatusId = updateDto.StatusId; // Cần thêm vào UserDtoRq nếu muốn cập nhật Status
                userToUpdate.IsDeleted = false; // Mặc định là false, cần thêm vào Dto nếu muốn điều khiển

                // Cập nhật các khóa ngoại:
                // Bạn có thể muốn kiểm tra xem DepartmentId, PositionId, ManagerUId có hợp lệ không
                // trước khi gán. Tôi sẽ thêm một ví dụ kiểm tra.
                if (updateDto.DepartmentId.HasValue)
                {
                    var departmentExists = await _unitOfWork.DepartmentRepository.AnyAsync(d => d.DepartmentId == updateDto.DepartmentId.Value);
                    if (!departmentExists)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "ID phòng ban không hợp lệ.",
                            code: "INVALID_DEPARTMENT_ID",
                            statusCode: 400
                        );
                    }
                    userToUpdate.DepartmentId = updateDto.DepartmentId;
                }


                if (updateDto.PositionId.HasValue)
                {
                    var positionExists = await _unitOfWork.PositionRepostiory.AnyAsync(p => p.PositionId == updateDto.PositionId.Value);
                    if (!positionExists)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "ID vị trí không hợp lệ.",
                            code: "INVALID_POSITION_ID",
                            statusCode: 400
                        );
                    }
                    userToUpdate.PositionId = updateDto.PositionId;
                }


                if (!string.IsNullOrEmpty(updateDto.ManagerUId))
                {
                    var managerExists = await _userManager.FindByIdAsync(updateDto.ManagerUId);
                    if (managerExists == null)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "ID người quản lý không hợp lệ.",
                            code: "INVALID_MANAGER_ID",
                            statusCode: 400
                        );
                    }
                    userToUpdate.ManagerUId = updateDto.ManagerUId;
                }


                userToUpdate.ModifedAt = DateTime.Now; // Cập nhật thời gian chỉnh sửa

                // 3. Cập nhật Email và Username (nếu thay đổi)
                // Lưu ý: UserDtoRq có Email là Required, nên luôn có giá trị
                if (!string.Equals(userToUpdate.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Kiểm tra xem email mới đã tồn tại cho người dùng khác chưa
                    var existingUserWithNewEmail = await _userManager.FindByEmailAsync(updateDto.Email);
                    if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != userToUpdate.Id)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "Email này đã được sử dụng bởi một tài khoản khác.",
                            code: "EMAIL_ALREADY_EXISTS",
                            statusCode: 400
                        );
                    }

                    var setEmailResult = await _userManager.SetEmailAsync(userToUpdate, updateDto.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            errors: setEmailResult.Errors.Select(e => e.Description),
                            code: "EMAIL_UPDATE_FAILED",
                            statusCode: 400
                        );
                    }
                    // Đồng bộ UserName với Email (thường là lowercase email)
                    var setUserNameResult = await _userManager.SetUserNameAsync(userToUpdate, updateDto.Email.ToLower());
                    if (!setUserNameResult.Succeeded)
                    {
                        return Result.Failure(
                           message: "Cập nhật người dùng thất bại",
                           errors: setUserNameResult.Errors.Select(e => e.Description),
                           code: "USERNAME_UPDATE_FAILED",
                           statusCode: 400
                       );
                    }
                }

                // 4. Cập nhật vai trò (nếu RoleId được cung cấp trong updateDto)
                if (!string.IsNullOrEmpty(updateDto.RoleId))
                {
                    var newRole = await _roleManager.FindByIdAsync(updateDto.RoleId);
                    if (newRole == null || string.IsNullOrEmpty(newRole.Name))
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "ID vai trò mới được cung cấp không hợp lệ hoặc không tồn tại.",
                            code: "INVALID_NEW_ROLE_ID",
                            statusCode: 400
                        );
                    }

                    var currentRoles = await _userManager.GetRolesAsync(userToUpdate);

                    // Loại bỏ tất cả các vai trò hiện tại
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(userToUpdate, currentRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            errors: removeRolesResult.Errors.Select(e => e.Description),
                            code: "REMOVE_ROLES_FAILED",
                            statusCode: 500
                        );
                    }

                    // Gán vai trò mới
                    var addRoleResult = await _userManager.AddToRoleAsync(userToUpdate, newRole.Name);
                    if (!addRoleResult.Succeeded)
                    {
                        // Nếu không thể thêm vai trò mới, bạn có thể cân nhắc gán lại các vai trò cũ ở đây
                        // hoặc chỉ log lỗi và trả về thất bại.
                        // Để an toàn, có thể thử add lại vai trò cũ:
                        // await _userManager.AddToRolesAsync(userToUpdate, currentRoles);
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            errors: addRoleResult.Errors.Select(e => e.Description),
                            code: "ADD_NEW_ROLE_FAILED",
                            statusCode: 500
                        );
                    }
                }
                // else { /* RoleId không được cung cấp, không thay đổi vai trò */ }

                // 5. Lưu các thay đổi vào database
                // UserManager.UpdateAsync đã tự động gọi SaveChanges của DbContext
                var updateResult = await _userManager.UpdateAsync(userToUpdate);
                if (!updateResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Cập nhật người dùng thất bại",
                        errors: updateResult.Errors.Select(e => e.Description),
                        code: "USER_UPDATE_FAILED",
                        statusCode: 500
                    );
                }

                return Result.Success(message: "Cập nhật người dùng thành công.", code: "USER_UPDATE_SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: ILogger)
                // _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", userId);
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
                     includes: q => q
                        .Include(d => d.Position),
                    asNoTracking: true

                );

                // 4. Calculate pagination metadata
                int effectiveLimit = queryParams.Limit;
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

        public async Task<Result<UserDto>> GetUserAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    predicate: u => u.Id == userId,
                    includes: q => q
                        .Include(d => d.Position)
                );
                if (user == null)
                {
                    return Result<UserDto>.Failure(
                  error: "Get user success!",
                  code: "SUCCESS",
                  statusCode: 404
                );
                }
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = roles.FirstOrDefault();

                return Result<UserDto>.Success(

                   message: "Get user success!",
                   code: "SUCCESS",
                   statusCode: 200,
                   data: userDto
               );

            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure(
                   error: ex.Message,
                   message: "An error occurred while retrieving the user list.",
                   code: "SYSTEM_ERROR",
                   statusCode: 500
               );
            }
        }

        public async Task<Result> ChangePasswordUserAsync(string userId, UserChangePasswordRq rq)
        {
            try
            {
                // 1. Tìm người dùng bằng userId (UserManager sẽ làm việc này hiệu quả hơn UserRepository cho Identity User)
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure(
                        error: "User not found.",
                        code: "USER_NOT_FOUND", // Đổi code rõ ràng hơn
                        statusCode: 404
                    );
                }

                // 2. Thay đổi mật khẩu bằng UserManager
                // UserManager.ChangePasswordAsync yêu cầu mật khẩu cũ để xác minh
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, rq.OldPassword, rq.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    // Mật khẩu đã được thay đổi thành công
                    // Bạn có thể không cần _unitOfWork.CompleteAsync() nếu UserManager đã xử lý việc lưu
                    return Result.Success(
                        message: "Password changed successfully!",
                        code: "SUCCESS",
                        statusCode: 200
                    // Không cần trả về userDto ở đây vì đây là hàm đổi mật khẩu
                    );
                }
                else
                {
                    // Mật khẩu không đổi được, lấy các lỗi từ IdentityResult
                    var errors = changePasswordResult.Errors.Select(e => e.Description);
                    var errorMessage = string.Join(" ", errors);

                    return Result.Failure(
                        error: errorMessage,
                        message: "Failed to change password. Please check your old password or new password requirements.",
                        code: "CHANGE_PASSWORD_FAILED",
                        statusCode: 400 // Bad Request vì lỗi từ phía người dùng (mật khẩu cũ sai, không đáp ứng yêu cầu)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ChangePasswordUserAsync: {ex}"); // Ghi log lỗi chi tiết
                return Result.Failure(
                    error: ex.Message,
                    message: "An unexpected error occurred while changing the password.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> ResetPasswordByAdminAsync(string userId, UserResetPasswordRq rq)
        {
            try
            {
                // 1. Tìm user theo ID
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure(
                        error: "User not found.",
                        code: "USER_NOT_FOUND",
                        statusCode: 404
                    );
                }

                // 2. Tạo token reset password (chuẩn của ASP.NET Identity)
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                // 3. Dùng token để reset password (mật khẩu cũ không cần)
                var resetResult = await _userManager.ResetPasswordAsync(user, resetToken, rq.NewPassword);

                if (resetResult.Succeeded)
                {
                    return Result.Success(
                        message: "Password reset successfully.",
                        code: "SUCCESS",
                        statusCode: 200
                    );
                }

                // 4. Xử lý lỗi nếu reset thất bại
                var errorMessages = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                return Result.Failure(
                    error: errorMessages,
                    message: "Failed to reset password.",
                    code: "RESET_PASSWORD_FAILED",
                    statusCode: 400
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ResetPasswordByAdminAsync: {ex}");
                return Result.Failure(
                    error: ex.Message,
                    message: "An unexpected error occurred while resetting the password.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }


        public async Task<Result<PagedResult<UserDto>>> SearchUserAsync(string keyword, BaseQueryParam queryParams)
        {
            try
            {
                IQueryable<ApplicationUser> query = _userManager.Users;

                // 1. Lọc theo tên (hoặc các trường khác nếu muốn)
                // Chuyển keyword về chữ thường để tìm kiếm không phân biệt hoa thường
                if (!string.IsNullOrEmpty(keyword))
                {
                    string lowerKeyword = keyword.ToLowerInvariant().Trim();
                    query = query.Where(u => u.UserName.ToLower().Contains(lowerKeyword) ||
                                             u.Email!.ToLower().Contains(lowerKeyword)); // Có thể tìm cả theo Email
                }

                // 2. Tính tổng số lượng bản ghi (trước khi phân trang và sắp xếp)
                int totalCount = await query.CountAsync();

                // 3. Sắp xếp
                // Mặc định sắp xếp theo CreatedAt giảm dần (hoặc UserName tăng dần, tùy ý)
                // Lưu ý: IdentityUser không có CreatedAt mặc định.
                // Nếu ApplicationUser của bạn có CreatedAt, hãy dùng nó.
                // Nếu không, có thể sắp xếp theo UserName hoặc Id.
                // 3. Sắp xếp (Chỉ CreatedAt)
                Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = q => q.OrderByDescending(u => u.CreatedAt); // Mặc định: CreatedAt DESC

                // Kiểm tra nếu có yêu cầu sắp xếp CreatedAt ASC
                if (queryParams.SortField?.ToLower() == "createdat")
                {
                    orderBy = queryParams.SortType?.ToLower() == "asc"
                        ? q => q.OrderBy(u => u.CreatedAt)
                        : q => q.OrderByDescending(u => u.CreatedAt);
                }
                // Nếu SortField không phải "createdat", hoặc không có, sẽ dùng mặc định là CreatedAt DESC.

                // Áp dụng sắp xếp
                query = orderBy(query);

                // 4. Phân trang
                int skip = (queryParams.Page - 1) * queryParams.Limit;
                query = query.Skip(skip).Take(queryParams.Limit);

                // 5. Lấy dữ liệu
                // ToListAsync() sẽ thực thi truy vấn
                var users = await query.ToListAsync();

                // 6. Ánh xạ từ ApplicationUser sang UserDto
                IEnumerable<UserDto> userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

                // 7. Tạo đối tượng PagedResult
                var pagination = new Pagination()
                {
                    CurrentPage = queryParams.Page,
                    ItemsPerPage = queryParams.Limit,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / queryParams.Limit)
                };



                var pagedResult = new PagedResult<UserDto>
                {
                    Items = userDtos,
                    Pagination = pagination
                };

                return Result<PagedResult<UserDto>>.Success(
                    data: pagedResult,
                    message: "User list retrieved successfully.",
                    code: "SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SearchUserAsync: {ex}"); // Ghi log chi tiết lỗi
                return Result<PagedResult<UserDto>>.Failure(
                    error: ex.Message,
                    message: "An error occurred while searching for users.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

    }
}
