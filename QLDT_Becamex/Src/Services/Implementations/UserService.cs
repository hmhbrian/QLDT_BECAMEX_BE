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
        private readonly CloudinaryService _cloudinaryService;

        public UserService(
            SignInManager<ApplicationUser> signInManager,
         UserManager<ApplicationUser> userManager,
         RoleManager<IdentityRole> roleManager,
         CloudinaryService cloudinaryService,
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
            _cloudinaryService = cloudinaryService;
        }




        public async Task<Result<UserDto>> LoginAsync(UserLoginRq loginDto)
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
                string targetRoleId;
                string targetRoleName;

                // 1. Xác định vai trò sẽ gán cho người dùng mới
                IdentityRole? selectedRole = null;

                if (!string.IsNullOrEmpty(registerDto.RoleId))
                {
                    // Kiểm tra RoleId có tồn tại trong DB không
                    selectedRole = await _roleManager.FindByIdAsync(registerDto.RoleId);
                    if (selectedRole == null)
                    {
                        return Result.Failure(
                            message: "Đăng ký thất bại",
                            error: $"Không tìm thấy vai trò với ID: {registerDto.RoleId}.",
                            code: "ROLE_NOT_FOUND",
                            statusCode: 400
                        );
                    }
                }
                else
                {
                    // Nếu không cung cấp RoleId, mặc định là "HOCVIEN"
                    selectedRole = await _roleManager.FindByNameAsync("HOCVIEN");
                    if (selectedRole == null)
                    {
                        return Result.Failure(
                            message: "Đăng ký thất bại",
                            error: "Vai trò mặc định 'HOCVIEN' không tồn tại.",
                            code: "DEFAULT_ROLE_MISSING",
                            statusCode: 500
                        );
                    }
                }

                // Gán thông tin vai trò
                targetRoleId = selectedRole.Id;
                targetRoleName = selectedRole.Name!;

                // 2. Kiểm tra email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Email này đã được sử dụng.",
                        code: "EMAIL_DUPLICATE",
                        statusCode: 400
                    );
                }

                // 3. Tạo người dùng mới
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
                    ManagerUId = registerDto.ManagerUId,
                    DepartmentId = registerDto.DepartmentId,
                    StatusId = registerDto.StatusId,
                    EndWork = registerDto.EndWork,
                };

                var createResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        errors: createResult.Errors.Select(e => e.Description),
                        code: "USER_CREATION_FAILED",
                        statusCode: 400
                    );
                }

                // 4. Gán vai trò cho user
                var addRoleResult = await _userManager.AddToRoleAsync(user, targetRoleName);
                if (!addRoleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user); // Rollback
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Không thể gán vai trò cho người dùng.",
                        code: "ROLE_ASSIGN_FAILED",
                        statusCode: 500
                    );
                }

                return Result.Success(message: "Đăng ký thành công.", code: "REGISTER_SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> UpdateUserByAdmin(string userId, AdminUpdateUserDtoRq rq)
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
                // Ánh xạ từ DTO sang entity (nếu bạn có AutoMapper mapping phù hợp)
                // Hoặc gán thủ công như bạn đang làm.
                // Cập nhật FullName nếu khác
                if (!string.IsNullOrWhiteSpace(rq.FullName) && !string.Equals(userToUpdate.FullName, rq.FullName, StringComparison.Ordinal))
                {
                    userToUpdate.FullName = rq.FullName;
                }

                // Cập nhật SĐT nếu khác
                if (!string.IsNullOrWhiteSpace(rq.NumberPhone) && !string.Equals(userToUpdate.PhoneNumber, rq.NumberPhone, StringComparison.Ordinal))
                {
                    userToUpdate.PhoneNumber = rq.NumberPhone;
                }

                // Cập nhật ngày bắt đầu nếu khác
                if (rq.StartWork.HasValue && userToUpdate.StartWork != rq.StartWork)
                {
                    userToUpdate.StartWork = rq.StartWork;
                }

                // Cập nhật ngày kết thúc nếu khác
                if (rq.EndWork.HasValue && userToUpdate.EndWork != rq.EndWork)
                {
                    userToUpdate.EndWork = rq.EndWork;
                }

                // Cập nhật trạng thái nếu khác
                if (rq.StatusId.HasValue && userToUpdate.StatusId != rq.StatusId)
                {
                    userToUpdate.StatusId = rq.StatusId;
                }

                // Cập nhật phòng ban nếu khác
                if (rq.DepartmentId.HasValue && userToUpdate.DepartmentId != rq.DepartmentId)
                {
                    userToUpdate.DepartmentId = rq.DepartmentId;
                }

                // Cập nhật ManagerUId nếu khác
                if (!string.IsNullOrEmpty(rq.ManagerUId) && !string.Equals(userToUpdate.ManagerUId, rq.ManagerUId, StringComparison.Ordinal))
                {
                    userToUpdate.ManagerUId = rq.ManagerUId;
                }

                if (rq.PositionId.HasValue && userToUpdate.PositionId != rq.PositionId)
                {
                    userToUpdate.PositionId = rq.PositionId;
                }

                // Nếu có thay đổi, cập nhật thời gian chỉnh sửa
                userToUpdate.ModifedAt = DateTime.UtcNow;

                // 3. Cập nhật Email và Username (nếu thay đổi)
                // Kiểm tra xem email mới có khác email hiện tại không
                if (!string.IsNullOrEmpty(rq.Email) && !string.Equals(userToUpdate.Email, rq.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Kiểm tra xem email mới đã tồn tại cho người dùng khác chưa
                    var existingUserWithNewEmail = await _userManager.FindByEmailAsync(rq.Email);
                    if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != userToUpdate.Id)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "Email này đã được sử dụng bởi một tài khoản khác.",
                            code: "EMAIL_ALREADY_EXISTS",
                            statusCode: 400
                        );
                    }

                    // Cập nhật Email và UserName thông qua UserManager
                    var setEmailResult = await _userManager.SetEmailAsync(userToUpdate, rq.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            errors: setEmailResult.Errors.Select(e => e.Description),
                            code: "EMAIL_UPDATE_FAILED",
                            statusCode: 400
                        );
                    }
                    // UserName thường được đồng bộ với Email (lowercase)
                    var setUserNameResult = await _userManager.SetUserNameAsync(userToUpdate, rq.Email.ToLowerInvariant());
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
                // else: Email không thay đổi hoặc rỗng, không làm gì.

                // 4. Kiểm tra và cập nhật CMND/CCCD (IdCard)
                if (!string.IsNullOrWhiteSpace(rq.IdCard) && !string.Equals(userToUpdate.IdCard, rq.IdCard, StringComparison.OrdinalIgnoreCase))
                {
                    var duplicateIdCardUser = await _userManager.Users
                        .Where(u => u.Id != userId && u.IdCard == rq.IdCard)
                        .FirstOrDefaultAsync();

                    if (duplicateIdCardUser != null)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "CMND/CCCD (IdCard) này đã tồn tại trong hệ thống.",
                            code: "IDCARD_ALREADY_EXISTS",
                            statusCode: 400
                        );
                    }
                    userToUpdate.IdCard = rq.IdCard; // Gán giá trị mới
                }


                // 5. Kiểm tra và cập nhật Mã nhân viên (Code)
                if (!string.IsNullOrWhiteSpace(rq.Code) && !string.Equals(userToUpdate.Code, rq.Code, StringComparison.OrdinalIgnoreCase))
                {
                    var duplicateCodeUser = await _userManager.Users
                        .Where(u => u.Id != userId && u.Code == rq.Code)
                        .FirstOrDefaultAsync();

                    if (duplicateCodeUser != null)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "Mã nhân viên (Code) này đã tồn tại trong hệ thống.",
                            code: "CODE_ALREADY_EXISTS",
                            statusCode: 400
                        );
                    }
                    userToUpdate.Code = rq.Code; // Gán giá trị mới
                }
                // Position


                // 6. Cập nhật vai trò
                if (!string.IsNullOrEmpty(rq.RoleId))
                {
                    var newRole = await _roleManager.FindByIdAsync(rq.RoleId);
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

                    // Chỉ thay đổi vai trò nếu vai trò mới khác với vai trò hiện tại
                    if (!currentRoles.Contains(newRole.Name))
                    {
                        // Xóa tất cả vai trò cũ
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

                        // Thêm vai trò mới
                        var addRoleResult = await _userManager.AddToRoleAsync(userToUpdate, newRole.Name);
                        if (!addRoleResult.Succeeded)
                        {
                            return Result.Failure(
                                message: "Cập nhật người dùng thất bại",
                                errors: addRoleResult.Errors.Select(e => e.Description),
                                code: "ADD_NEW_ROLE_FAILED",
                                statusCode: 500
                            );
                        }
                    }
                }
                // else: RoleId không được cung cấp, không thay đổi vai trò.


                // 7. Thay đổi mật khẩu (chỉ nếu rq.Password được cung cấp và không rỗng)
                if (!string.IsNullOrWhiteSpace(rq.NewPassword))
                {
                    // Tạo token reset password (UserManager sẽ kiểm tra tính hợp lệ của token)
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userToUpdate);

                    // Sử dụng token để reset password (không cần mật khẩu cũ)
                    var resetResult = await _userManager.ResetPasswordAsync(userToUpdate, resetToken, rq.NewPassword);

                    if (!resetResult.Succeeded)
                    {
                        var errorMessages = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                        return Result.Failure(
                            error: errorMessages,
                            message: "Cập nhật người dùng thất bại: Không thể đặt lại mật khẩu mới.",
                            code: "RESET_PASSWORD_FAILED",
                            statusCode: 400 // Mật khẩu mới không đáp ứng yêu cầu
                        );
                    }
                }

                // 8. Lưu các thay đổi cuối cùng vào database
                // UserManager.UpdateAsync sẽ lưu các thay đổi của các thuộc tính trực tiếp trên userToUpdate
                // Các thao tác SetEmail/UserName, AddToRole/RemoveFromRoles, ResetPasswordAsync đã tự động lưu.
                // Tuy nhiên, việc gọi UpdateAsync này cũng đảm bảo những thay đổi đơn giản khác được lưu.
                var updateResult = await _userManager.UpdateAsync(userToUpdate);
                if (!updateResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Cập nhật người dùng thất bại",
                        errors: updateResult.Errors.Select(e => e.Description),
                        code: "USER_FINAL_UPDATE_FAILED", // Đổi code cho rõ ràng
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
                    message: "Đã xảy ra lỗi hệ thống khi cập nhật người dùng. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public async Task<Result> UpdateMyProfileAsync(string userId, UserUpdateSelfDtoRq rq)
        {
            try
            {
                // 1. Tìm người dùng cần cập nhật
                var userToUpdate = await _userManager.FindByIdAsync(userId);
                if (userToUpdate == null)
                {
                    return Result.Failure(
                        message: "Cập nhật thông tin cá nhân thất bại",
                        error: $"Không tìm thấy người dùng với ID: {userId}.",
                        code: "USER_NOT_FOUND",
                        statusCode: 404
                    );
                }

                string? imageUrl = null;
                if (rq.UrlAvatar != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(rq.UrlAvatar);
                }

                // Cập nhật các trường nếu có dữ liệu mới
                if (!string.IsNullOrWhiteSpace(rq.FullName))
                    userToUpdate.FullName = rq.FullName;

                if (!string.IsNullOrWhiteSpace(rq.PhoneNumber))
                    userToUpdate.PhoneNumber = rq.PhoneNumber;

                if (!string.IsNullOrWhiteSpace(imageUrl))
                    userToUpdate.UrlAvatar = imageUrl;

                await _unitOfWork.CompleteAsync();

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

                foreach (var userDto in userDtos)
                {
                    var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        userDto.Role = roles.FirstOrDefault();
                    }
                }

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

                foreach (var userDto in userDtos)
                {
                    var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        userDto.Role = roles.FirstOrDefault();
                    }
                }

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
