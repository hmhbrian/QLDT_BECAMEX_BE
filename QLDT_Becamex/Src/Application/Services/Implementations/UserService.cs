using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Shared.Helpers;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý người dùng.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IJwtService _jwtService;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="UserService"/>.
        /// </summary>
        /// <param name="signInManager">Đối tượng SignInManager để quản lý việc đăng nhập người dùng.</param>
        /// <param name="userManager">Đối tượng UserManager để quản lý người dùng.</param>
        /// <param name="roleManager">Đối tượng RoleManager để quản lý vai trò.</param>
        /// <param name="cloudinaryService">Dịch vụ Cloudinary để tải ảnh lên.</param>
        /// <param name="jwtService">Dịch vụ JWT để tạo token.</param>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        /// <param name="unitOfWork">Đối tượng Unit of Work để quản lý các repositories và giao dịch cơ sở dữ liệu.</param>
        /// <param name="httpContextAccessor">Đối tượng HttpContextAccessor để truy cập HttpContext.</param>
        public UserService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ICloudinaryService cloudinaryService,
            IJwtService jwtService,
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
            _jwtService = jwtService;
        }

        /// <summary>
        /// Tạo một người dùng mới.
        /// </summary>
        /// <param name="registerDto">Đối tượng chứa thông tin đăng ký người dùng.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> CreateUserAsync(UserCreateDto registerDto)
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
                            code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                            code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
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
                        code: "EXISTS", // Dựa vào Status Code 409: Xung đột dữ liệu
                        statusCode: 409
                    );
                }

                // Tìm IdentityRole bằng RoleId
                var role = await _roleManager.FindByIdAsync(targetRoleId);
                if (role == null || string.IsNullOrEmpty(role.Name))
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        error: "Vai trò liên kết với vị trí không tồn tại hoặc không hợp lệ.",
                        code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                    DepartmentId = registerDto.DepartmentId,
                    PositionId = registerDto.PositionId,
                    StatusId = registerDto.StatusId,
                    ManagerUId = registerDto.ManagerUId
                    // DepartmentId và ManagerId (nếu có trong RegisterDto, cần thêm vào)
                };

                var createUserResult = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createUserResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Đăng ký thất bại",
                        errors: createUserResult.Errors.Select(e => e.Description),
                        code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ (lỗi validation mật khẩu, v.v.)
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
                        code: "FORBIDDEN", // Giả định là lỗi liên quan đến quyền của role
                        statusCode: 400
                    );
                }

                return Result.Success(message: "Đăng ký thành công.", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: ILogger)
                // Console.WriteLine($"Error registering user: {ex.Message}");
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }



        /// <summary>
        /// Xử lý yêu cầu đăng nhập của người dùng.
        /// </summary>
        /// <param name="loginDto">Đối tượng chứa thông tin đăng nhập.</param>
        /// <returns>Đối tượng Result chứa thông tin người dùng nếu đăng nhập thành công hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<UserDto>> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                // Hãy đảm bảo rằng LoginDto.Email thực sự chứa email, và bạn tìm theo email.
                var user = await _userManager.Users
                               .Include(u => u.Position)
                               .Include(u => u.Department)
                               .Include(u => u.ManagerU)
                               .Include(u => u.UserStatus)
                               .FirstOrDefaultAsync(u => u.Email == loginDto.Email);
                // Nếu không tìm thấy bằng email, bạn có thể cân nhắc tìm bằng username nếu muốn linh hoạt

                if (user == null)
                {
                    // Trả về lỗi chung để tránh lộ thông tin liệu email/username có tồn tại hay không.
                    return Result<UserDto>.Failure(
                        message: "Đăng nhập thất bại",
                        error: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                        code: "UNAUTHORIZED", // Dựa vào Status Code 401: Lỗi xác thực
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

                    string id = userDto?.Id!;
                    string email = userDto?.Email!;
                    string role = userDto?.Role!;

                    string accessToken = _jwtService.GenerateJwtToken(id, email, role);

                    userDto.AccessToken = accessToken;

                    // Trả về Result.Success với UserDto
                    return Result<UserDto>.Success(
                        message: "Đăng nhập thành công.",
                        code: "SUCCESS", // Dựa vào Status Code 200: Thành công
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
                        code: "FORBIDDEN", // Dựa vào Status Code 403: Cấm, phân quyền
                        statusCode: 403 // Forbidden
                    );
                }
                else if (result.IsNotAllowed)
                {
                    // Đăng nhập không được phép (ví dụ: email chưa xác nhận)
                    return Result<UserDto>.Failure(
                        message: "Đăng nhập thất bại",
                        error: "Đăng nhập không được phép. Tài khoản của bạn chưa được xác nhận hoặc có vấn đề.",
                        code: "FORBIDDEN", // Dựa vào Status Code 403: Cấm, phân quyền
                        statusCode: 403 // Forbidden hoặc 400 Bad Request, tùy ngữ cảnh
                    );
                }
                else
                {
                    // Các trường hợp thất bại khác (ví dụ: sai mật khẩu)
                    return Result<UserDto>.Failure(
                        message: "Đăng nhập thất bại",
                        error: "Tên đăng nhập hoặc mật khẩu không chính xác.",
                        code: "UNAUTHORIZED", // Dựa vào Status Code 401: Lỗi xác thực
                        statusCode: 401 // Unauthorized
                    );
                }
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }


        /// <summary>
        /// Cập nhật thông tin người dùng bởi quản trị viên.
        /// </summary>
        /// <param name="userId">ID của người dùng cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> UpdateUserByAdmin(string userId, UserAdminUpdateDto rq)
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
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
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
                userToUpdate.ModifiedAt = DateTime.UtcNow;

                // 3. Cập nhật Email và Username (nếu thay đổi)
                // Kiểm tra xem email mới có khác email hiện tại không
                if (!string.IsNullOrEmpty(rq.Email) && !string.Equals(userToUpdate.Email, rq.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Kiểm tra xem email mới đã tồn tại cho người dùng khác chưa
                    var existingUserWithNewEmail = await _userManager.Users
                        .Where(u => u.Id != userId && u.Email == rq.Email)
                        .FirstOrDefaultAsync();

                    if (existingUserWithNewEmail != null)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            error: "Email này đã được sử dụng bởi một tài khoản khác.",
                            code: "EXISTS", // Dựa vào Status Code 409: Xung đột dữ liệu
                            statusCode: 409
                        );
                    }

                    // Cập nhật Email và UserName thông qua UserManager
                    var setEmailResult = await _userManager.SetEmailAsync(userToUpdate, rq.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        return Result.Failure(
                            message: "Cập nhật người dùng thất bại",
                            errors: setEmailResult.Errors.Select(e => e.Description),
                            code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                            code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                            code: "EXISTS", // Dựa vào Status Code 409: Xung đột dữ liệu
                            statusCode: 409
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
                            code: "EXISTS", // Dựa vào Status Code 409: Xung đột dữ liệu
                            statusCode: 409
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
                            code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                                code: "FORBIDDEN", // Dựa vào Status Code 403: Cấm, phân quyền
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
                                code: "FORBIDDEN", // Dựa vào Status Code 403: Cấm, phân quyền
                                statusCode: 500
                            );
                        }
                    }
                }
                // else: RoleId không được cung cấp, không thay đổi vai trò.

                // 7. Thay đổi mật khẩu (chỉ nếu rq.NewPassword được cung cấp và không rỗng)
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
                            code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
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
                        code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                        statusCode: 500
                    );
                }

                return Result.Success(message: "Cập nhật người dùng thành công.", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: ILogger)
                // _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", userId);
                return Result.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi hệ thống khi cập nhật người dùng. Vui lòng thử lại sau.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật thông tin hồ sơ cá nhân của người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin cập nhật hồ sơ cá nhân.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> UpdateMyProfileAsync(string userId, UserUserUpdateDto rq)
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
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
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

                // Cần gọi UserManager.UpdateAsync để lưu thay đổi vào database
                var updateResult = await _userManager.UpdateAsync(userToUpdate);
                if (!updateResult.Succeeded)
                {
                    return Result.Failure(
                        message: "Cập nhật thông tin cá nhân thất bại",
                        errors: updateResult.Errors.Select(e => e.Description),
                        code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                        statusCode: 500
                    );
                }

                return Result.Success(message: "Cập nhật người dùng thành công.", code: "SUCCESS", statusCode: 200);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết tại đây (ví dụ: ILogger)
                // _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", userId);
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Thực hiện xóa mềm (soft delete) một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng cần xóa mềm.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
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
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
                        statusCode: 404 // Not Found
                    );
                }

                // 2. Kiểm tra nếu người dùng đã bị xóa mềm rồi
                if (user.IsDeleted)
                {
                    return Result.Failure(
                        message: "Xóa người dùng thất bại",
                        error: "Người dùng này đã bị xóa rồi.",
                        code: "EXISTS", // Dựa vào Status Code 409: Xung đột dữ liệu (đã tồn tại trạng thái đã xóa)
                        statusCode: 409 // Bad Request hoặc Conflict
                    );
                }

                // 3. Thực hiện xóa mềm: Cập nhật IsDeleted thành true và đặt ModifiedAt
                user.IsDeleted = true;
                user.ModifiedAt = DateTime.UtcNow; // Cập nhật cả UpdatedAt

                // 4. Cập nhật người dùng trong UserManager
                // Lưu ý: UserManager.UpdateAsync sẽ lưu các thay đổi này vào DB
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result.Failure(
                        message: "Xóa người dùng thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                        statusCode: 500 // Internal Server Error
                    );
                }

                return Result.Success(
                    message: "Xóa người dùng thành công (soft delete).", // Cập nhật thông báo
                    code: "SUCCESS",
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy thông tin xác thực của người dùng hiện tại từ HttpContext.
        /// </summary>
        /// <returns>Một tuple chứa ID người dùng và vai trò người dùng, hoặc null nếu không tìm thấy.</returns>
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;

            var userId = currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = currentUser?.FindFirst(ClaimTypes.Role)?.Value;


            return (userId, role);
        }

        /// <summary>
        /// Lấy danh sách người dùng đã được phân trang và lọc.
        /// </summary>
        /// <param name="queryParams">Tham số truy vấn bao gồm phân trang và sắp xếp.</param>
        /// <returns>Đối tượng Result chứa danh sách người dùng đã phân trang hoặc lỗi.</returns>
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
                        .Include(d => d.Position)
                        .Include(d => d.Department)
                        .Include(d => d.ManagerU)
                        .Include(d => d.UserStatus),
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
                    message: "Lấy danh sách người dùng thành công.",
                    code: "SUCCESS", // Dựa vào Status Code 200: Thành công
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                // Log the exception details
                return Result<PagedResult<UserDto>>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi truy xuất danh sách người dùng.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng cần lấy thông tin.</param>
        /// <returns>Đối tượng Result chứa thông tin người dùng hoặc lỗi nếu không tìm thấy.</returns>
        public async Task<Result<UserDto>> GetUserAsync(string userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                    predicate: u => u.Id == userId,
                    includes: q => q
                        .Include(d => d.Position)
                        .Include(d => d.Department)
                        .Include(d => d.ManagerU)
                        .Include(d => d.UserStatus)
                );
                if (user == null)
                {
                    return Result<UserDto>.Failure(
                        error: "Không tìm thấy người dùng.",
                        message: "Lấy thông tin người dùng thất bại.",
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
                        statusCode: 404
                    );
                }
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Role = roles.FirstOrDefault();

                return Result<UserDto>.Success(
                    message: "Lấy thông tin người dùng thành công.",
                    code: "SUCCESS", // Dựa vào Status Code 200: Thành công
                    statusCode: 200,
                    data: userDto
                );
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi truy xuất thông tin người dùng.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Đổi mật khẩu của người dùng.
        /// </summary>
        /// <param name="userId">ID của người dùng cần đổi mật khẩu.</param>
        /// <param name="rq">Đối tượng chứa mật khẩu cũ và mật khẩu mới.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> ChangePasswordUserAsync(string userId, UserChangePasswordDto rq)
        {
            try
            {
                // 1. Tìm người dùng bằng userId (UserManager sẽ làm việc này hiệu quả hơn UserRepository cho Identity User)
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure(
                        error: "Không tìm thấy người dùng.",
                        message: "Đổi mật khẩu thất bại.",
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
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
                        message: "Đổi mật khẩu thành công!",
                        code: "SUCCESS", // Dựa vào Status Code 200: Thành công
                        statusCode: 200
                    );
                }
                else
                {
                    // Mật khẩu không đổi được, lấy các lỗi từ IdentityResult
                    var errors = changePasswordResult.Errors.Select(e => e.Description);
                    var errorMessage = string.Join(" ", errors);

                    return Result.Failure(
                        error: errorMessage,
                        message: "Thay đổi mật khẩu thất bại. Vui lòng kiểm tra mật khẩu cũ hoặc yêu cầu về mật khẩu mới.",
                        code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
                        statusCode: 400 // Bad Request vì lỗi từ phía người dùng (mật khẩu cũ sai, không đáp ứng yêu cầu)
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ChangePasswordUserAsync: {ex}"); // Ghi log lỗi chi tiết
                return Result.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi không mong muốn khi đổi mật khẩu.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Đặt lại mật khẩu của người dùng bởi quản trị viên.
        /// </summary>
        /// <param name="userId">ID của người dùng cần đặt lại mật khẩu.</param>
        /// <param name="rq">Đối tượng chứa mật khẩu mới.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<Result> ResetPasswordByAdminAsync(string userId, UserResetPasswordDto rq)
        {
            try
            {
                // 1. Tìm user theo ID
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Result.Failure(
                        error: "Không tìm thấy người dùng.",
                        code: "NOT_FOUND", // Dựa vào Status Code 404: Không tìm thấy
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
                        message: "Đặt lại mật khẩu thành công.",
                        code: "SUCCESS", // Dựa vào Status Code 200: Thành công
                        statusCode: 200
                    );
                }

                // 4. Xử lý lỗi nếu reset thất bại
                var errorMessages = string.Join(" ", resetResult.Errors.Select(e => e.Description));
                return Result.Failure(
                    error: errorMessages,
                    message: "Đặt lại mật khẩu thất bại.",
                    code: "INVALID", // Dựa vào Status Code 400: Dữ liệu đầu vào không hợp lệ
                    statusCode: 400
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] ResetPasswordByAdminAsync: {ex}");
                return Result.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi không mong muốn khi đặt lại mật khẩu.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo từ khóa và phân trang.
        /// </summary>
        /// <param name="keyword">Từ khóa để tìm kiếm (tên đầy đủ hoặc email).</param>
        /// <param name="queryParams">Tham số truy vấn bao gồm phân trang và sắp xếp.</param>
        /// <returns>Đối tượng Result chứa danh sách người dùng đã tìm thấy và thông tin phân trang.</returns>
        public async Task<Result<PagedResult<UserDto>>> SearchUserAsync(string keyword, BaseQueryParam queryParams)
        {
            try
            {
                // 1. Lấy tất cả user (chưa phân trang)
                var users = await _userManager.Users
                    .Where(u => !u.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync();

                // 2. Tìm kiếm không dấu (in-memory)
                if (!string.IsNullOrEmpty(keyword))
                {
                    string normalizedKeyword = StringHelper.RemoveDiacritics(keyword).ToLowerInvariant().Trim();

                    users = users.Where(u =>
                        StringHelper.RemoveDiacritics(u.FullName ?? "").ToLower().Contains(normalizedKeyword) ||
                        (u.Email ?? "").ToLower().Contains(normalizedKeyword)
                    ).ToList();
                }

                // 3. Tổng số kết quả sau lọc
                int totalCount = users.Count;

                // 4. Sắp xếp
                if (queryParams.SortField?.ToLower() == "createdat")
                {
                    users = queryParams.SortType?.ToLower() == "asc"
                        ? users.OrderBy(u => u.CreatedAt).ToList()
                        : users.OrderByDescending(u => u.CreatedAt).ToList();
                }
                else
                {
                    users = users.OrderByDescending(u => u.CreatedAt).ToList(); // Mặc định
                }

                // 5. Phân trang
                int skip = (queryParams.Page - 1) * queryParams.Limit;
                var pagedUsers = users.Skip(skip).Take(queryParams.Limit).ToList();

                // 6. Map sang DTO
                var userDtos = _mapper.Map<List<UserDto>>(pagedUsers);

                // 7. Gán Role cho từng UserDto
                foreach (var userDto in userDtos)
                {
                    var user = pagedUsers.FirstOrDefault(u => u.Id == userDto.Id);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        userDto.Role = roles.FirstOrDefault();
                    }
                }

                // 8. Kết quả phân trang
                var pagination = new Pagination()
                {
                    CurrentPage = queryParams.Page,
                    ItemsPerPage = queryParams.Limit,
                    TotalItems = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / queryParams.Limit)
                };

                return Result<PagedResult<UserDto>>.Success(
                    data: new PagedResult<UserDto>
                    {
                        Items = userDtos,
                        Pagination = pagination
                    },
                    message: "Lấy danh sách người dùng thành công.",
                    code: "SUCCESS", // Dựa vào Status Code 200: Thành công
                    statusCode: 200
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SearchUserAsync: {ex}");
                return Result<PagedResult<UserDto>>.Failure(
                    error: ex.Message,
                    message: "Đã xảy ra lỗi khi tìm kiếm người dùng.",
                    code: "SYSTEM_ERROR", // Dựa vào Status Code 500: Lỗi chung liên quan đến network, database
                    statusCode: 500
                );
            }
        }
    }
}