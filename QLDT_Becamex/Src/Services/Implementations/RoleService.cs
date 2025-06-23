using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Services.Interfaces;
using System; // Thêm để sử dụng Exception
using System.Collections.Generic; // Thêm để sử dụng IEnumerable và List
using System.Linq;
using QLDT_Becamex.Src.Dtos; // Thêm để sử dụng LINQ

namespace QLDT_Becamex.Src.Services.Implementations
{
    /// <summary>
    /// Triển khai dịch vụ quản lý vai trò.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager; // Hoặc RoleManager<ApplicationRole>
        private readonly IMapper _mapper;

        /// <summary>
        /// Khởi tạo một phiên bản mới của lớp <see cref="RoleService"/>.
        /// </summary>
        /// <param name="roleManager">Đối tượng RoleManager để quản lý vai trò.</param>
        /// <param name="mapper">Đối tượng AutoMapper để ánh xạ giữa các đối tượng.</param>
        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo một vai trò mới.
        /// </summary>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu tạo vai trò.</param>
        /// <returns>Đối tượng Result chứa thông tin vai trò đã tạo hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<RoleDto>> CreateRoleAsync(RoleRq rq)
        {
            try
            {
                // 1. Kiểm tra xem vai trò đã tồn tại chưa
                var roleExists = await _roleManager.RoleExistsAsync(rq.RoleName);
                if (roleExists)
                {
                    return Result<RoleDto>.Failure(
                        message: "Tạo vai trò thất bại",
                        error: $"Vai trò '{rq.RoleName}' đã tồn tại.",
                        code: "EXISTS", // Thay đổi mã lỗi theo bảng: ROLE_ALREADY_EXISTS -> EXISTS
                        statusCode: 409 // 409: Xung đột dữ liệu
                    );
                }

                // 2. Tạo đối tượng IdentityRole
                var role = _mapper.Map<IdentityRole>(rq);
                // Nếu bạn có ApplicationRole với CreatedAt, bạn có thể gán ở đây:
                // if (role is ApplicationRole appRole) appRole.CreatedAt = DateTime.UtcNow;

                // 3. Tạo vai trò bằng RoleManager
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    var roleDto = _mapper.Map<RoleDto>(role);
                    return Result<RoleDto>.Success(
                        message: "Tạo vai trò thành công.",
                        code: "SUCCESS", // Thay đổi mã lỗi theo bảng: CREATE_ROLE_SUCCESS -> SUCCESS
                        statusCode: 201,
                        data: roleDto
                    );
                }
                else
                {
                    return Result<RoleDto>.Failure(
                        message: "Tạo vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: CREATE_ROLE_FAILED -> INVALID (dữ liệu đầu vào không hợp lệ)
                        statusCode: 400
                    );
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result<RoleDto>.Failure(
                    message: "Tạo vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy thông tin vai trò theo ID.
        /// </summary>
        /// <param name="roleId">ID của vai trò cần lấy.</param>
        /// <returns>Đối tượng Result chứa thông tin vai trò hoặc lỗi nếu không tìm thấy.</returns>
        public async Task<Result<RoleDto>> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Result<RoleDto>.Failure(
                        message: "Lấy thông tin vai trò thất bại",
                        error: "Vai trò không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: ROLE_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }
                var roleDto = _mapper.Map<RoleDto>(role);
                return Result<RoleDto>.Success(
                    message: "Lấy thông tin vai trò thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_ROLE_SUCCESS -> SUCCESS
                    statusCode: 200,
                    data: roleDto
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result<RoleDto>.Failure(
                    message: "Lấy thông tin vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy thông tin vai trò theo tên.
        /// </summary>
        /// <param name="roleName">Tên của vai trò cần lấy.</param>
        /// <returns>Đối tượng Result chứa thông tin vai trò hoặc lỗi nếu không tìm thấy.</returns>
        public async Task<Result<RoleDto>> GetRoleByNameAsync(string roleName)
        {
            try
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    return Result<RoleDto>.Failure(
                        message: "Lấy thông tin vai trò thất bại",
                        error: "Vai trò không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: ROLE_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }
                var roleDto = _mapper.Map<RoleDto>(role);
                return Result<RoleDto>.Success(
                    message: "Lấy thông tin vai trò thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_ROLE_SUCCESS -> SUCCESS
                    statusCode: 200,
                    data: roleDto
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result<RoleDto>.Failure(
                    message: "Lấy thông tin vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Lấy tất cả các vai trò.
        /// </summary>
        /// <returns>Đối tượng Result chứa danh sách các vai trò hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = _roleManager.Roles.ToList(); // Lấy tất cả các vai trò từ RoleManager
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return Result<IEnumerable<RoleDto>>.Success(
                    message: "Lấy danh sách vai trò thành công.",
                    code: "SUCCESS", // Thay đổi mã lỗi theo bảng: GET_ALL_ROLES_SUCCESS -> SUCCESS
                    statusCode: 200,
                    data: roleDtos
                );
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result<IEnumerable<RoleDto>>.Failure(
                    message: "Lấy danh sách vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Cập nhật thông tin một vai trò hiện có.
        /// </summary>
        /// <param name="roleId">ID của vai trò cần cập nhật.</param>
        /// <param name="rq">Đối tượng chứa thông tin yêu cầu cập nhật vai trò.</param>
        /// <returns>Đối tượng Result chứa thông tin vai trò đã cập nhật hoặc lỗi nếu thất bại.</returns>
        public async Task<Result<RoleDto>> UpdateRoleAsync(string roleId, RoleRq rq)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Result<RoleDto>.Failure(
                        message: "Cập nhật vai trò thất bại",
                        error: "Vai trò không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: ROLE_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra xem tên vai trò mới có trùng với vai trò khác (không phải vai trò đang cập nhật)
                if (!string.Equals(role.Name, rq.RoleName, StringComparison.OrdinalIgnoreCase))
                {
                    var roleWithSameName = await _roleManager.FindByNameAsync(rq.RoleName);
                    if (roleWithSameName != null && roleWithSameName.Id != roleId)
                    {
                        return Result<RoleDto>.Failure(
                            message: "Cập nhật vai trò thất bại",
                            error: $"Tên vai trò '{rq.RoleName}' đã được sử dụng bởi vai trò khác.",
                            code: "EXISTS", // Thay đổi mã lỗi theo bảng: ROLE_NAME_ALREADY_EXISTS -> EXISTS
                            statusCode: 409 // 409: Xung đột dữ liệu
                        );
                    }
                }

                // Cập nhật thuộc tính của role
                role.Name = rq.RoleName;
                role.NormalizedName = rq.RoleName.ToUpper();
                // Nếu bạn có ApplicationRole với UpdatedAt, bạn có thể gán ở đây:
                // if (role is ApplicationRole appRole) appRole.UpdatedAt = DateTime.UtcNow;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    var roleDto = _mapper.Map<RoleDto>(role);
                    return Result<RoleDto>.Success(
                        message: "Cập nhật vai trò thành công.",
                        code: "SUCCESS", // Thay đổi mã lỗi theo bảng: UPDATE_ROLE_SUCCESS -> SUCCESS
                        statusCode: 200,
                        data: roleDto
                    );
                }
                else
                {
                    return Result<RoleDto>.Failure(
                        message: "Cập nhật vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: UPDATE_ROLE_FAILED -> INVALID (dữ liệu đầu vào không hợp lệ)
                        statusCode: 400
                    );
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result<RoleDto>.Failure(
                    message: "Cập nhật vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }

        /// <summary>
        /// Xóa một vai trò.
        /// </summary>
        /// <param name="roleId">ID của vai trò cần xóa.</param>
        /// <returns>Đối tượng Result cho biết kết quả của thao tác.</returns>
        public async Task<ApiResponse> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse.Failure(
                        message: "Xóa vai trò thất bại",
                        error: "Vai trò không tồn tại.",
                        code: "NOT_FOUND", // Thay đổi mã lỗi theo bảng: ROLE_NOT_FOUND -> NOT_FOUND
                        statusCode: 404
                    );
                }

                // Kiểm tra xem có người dùng nào đang giữ vai trò này không (Tùy chọn)
                // Đây là một kiểm tra phức tạp hơn và có thể yêu cầu truy vấn UserManager.
                // Để đơn giản, chúng ta sẽ không thực hiện kiểm tra này ở đây.
                // Nếu có người dùng, DeleteAsync sẽ báo lỗi nếu có ràng buộc khóa ngoại.

                var result = await _roleManager.DeleteAsync(role); // Xóa cứng vai trò

                if (result.Succeeded)
                {
                    return ApiResponse.Success(
                        message: "Xóa vai trò thành công.",
                        code: "SUCCESS", // Thay đổi mã lỗi theo bảng: DELETE_ROLE_SUCCESS -> SUCCESS
                        statusCode: 200
                    );
                }
                else
                {
                    return ApiResponse.Failure(
                        message: "Xóa vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "INVALID", // Thay đổi mã lỗi theo bảng: DELETE_ROLE_FAILED -> INVALID (có thể do ràng buộc)
                        statusCode: 400
                    );
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return ApiResponse.Failure(
                    message: "Xóa vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR", // Mã lỗi chung: SYSTEM_ERROR
                    statusCode: 500
                );
            }
        }
    }
}