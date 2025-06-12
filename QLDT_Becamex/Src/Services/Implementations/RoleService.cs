using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos.Roles;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.Dtos.Results;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager; // Hoặc RoleManager<ApplicationRole>
        private readonly IMapper _mapper;

        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        // --- CREATE ---
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
                        code: "ROLE_ALREADY_EXISTS",
                        statusCode: 400
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
                        code: "CREATE_ROLE_SUCCESS",
                        statusCode: 201,
                        data: roleDto
                    );
                }
                else
                {
                    return Result<RoleDto>.Failure(
                        message: "Tạo vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "CREATE_ROLE_FAILED",
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
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- READ by ID ---
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
                        code: "ROLE_NOT_FOUND",
                        statusCode: 404
                    );
                }
                var roleDto = _mapper.Map<RoleDto>(role);
                return Result<RoleDto>.Success(
                    message: "Lấy thông tin vai trò thành công.",
                    code: "GET_ROLE_SUCCESS",
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
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- READ by Name ---
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
                        code: "ROLE_NOT_FOUND",
                        statusCode: 404
                    );
                }
                var roleDto = _mapper.Map<RoleDto>(role);
                return Result<RoleDto>.Success(
                    message: "Lấy thông tin vai trò thành công.",
                    code: "GET_ROLE_SUCCESS",
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
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- READ All ---
        public async Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync()
        {
            try
            {
                var roles = _roleManager.Roles.ToList(); // Lấy tất cả các vai trò từ RoleManager
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return Result<IEnumerable<RoleDto>>.Success(
                    message: "Lấy danh sách vai trò thành công.",
                    code: "GET_ALL_ROLES_SUCCESS",
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
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- UPDATE ---
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
                        code: "ROLE_NOT_FOUND",
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
                            code: "ROLE_NAME_ALREADY_EXISTS",
                            statusCode: 400
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
                        code: "UPDATE_ROLE_SUCCESS",
                        statusCode: 200,
                        data: roleDto
                    );
                }
                else
                {
                    return Result<RoleDto>.Failure(
                        message: "Cập nhật vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "UPDATE_ROLE_FAILED",
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
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        // --- DELETE ---
        public async Task<Result> DeleteRoleAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return Result.Failure(
                        message: "Xóa vai trò thất bại",
                        error: "Vai trò không tồn tại.",
                        code: "ROLE_NOT_FOUND",
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
                    return Result.Success(
                        message: "Xóa vai trò thành công.",
                        code: "DELETE_ROLE_SUCCESS",
                        statusCode: 200
                    );
                }
                else
                {
                    return Result.Failure(
                        message: "Xóa vai trò thất bại",
                        errors: result.Errors.Select(e => e.Description),
                        code: "DELETE_ROLE_FAILED",
                        statusCode: 400
                    );
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                return Result.Failure(
                    message: "Xóa vai trò thất bại",
                    error: $"Đã xảy ra lỗi hệ thống: {ex.Message}",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}
