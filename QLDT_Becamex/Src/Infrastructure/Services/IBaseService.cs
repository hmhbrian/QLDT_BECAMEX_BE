using AutoMapper;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public interface IBaseService
    {
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo();
        public Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId);
        public Task ValidateManagerIdDeparmentAsync(string? managerId, bool isRequired, string? currentManagerId, int? departmentId);
        public Task<DepartmentDto> MapToDtoAsync(
            Department dept,
            Dictionary<int, Department> departmentDict,
            Dictionary<string, ApplicationUser> userDict,
            Dictionary<int, List<string>> pathCache,
            IMapper mapper);
    }
}
