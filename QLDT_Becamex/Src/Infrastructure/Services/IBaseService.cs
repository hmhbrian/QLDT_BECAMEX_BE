namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface IBaseService
    {
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
        public Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId);
        public Task ValidateManagerIdDeparmentAsync(string? managerId, bool isRequired, string? currentManagerId, int? departmentId);
    }
}
