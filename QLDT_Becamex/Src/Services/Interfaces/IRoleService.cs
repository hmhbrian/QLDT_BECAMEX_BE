using QLDT_Becamex.Src.Dtos;

namespace QLDT_Becamex.Src.Services.Interfaces
{
    public interface IRoleService
    {
        // Create
        public Task<Result<RoleDto>> CreateRoleAsync(RoleRq rq);

        // Read
        public Task<Result<RoleDto>> GetRoleByIdAsync(string roleId);
        public Task<Result<RoleDto>> GetRoleByNameAsync(string roleName);
        public Task<Result<IEnumerable<RoleDto>>> GetAllRolesAsync();

        // Update
        public Task<Result<RoleDto>> UpdateRoleAsync(string roleId, RoleRq rq);

        // Delete
        public Task<ApiResponse> DeleteRoleAsync(string roleId);
    }
}
