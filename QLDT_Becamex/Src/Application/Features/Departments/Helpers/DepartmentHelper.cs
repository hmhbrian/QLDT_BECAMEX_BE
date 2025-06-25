using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;

namespace QLDT_Becamex.Src.Application.Features.Departments.Helpers
{
    public static class DepartmentHelper
    {
        public static async Task ValidateManagerIdAsync(string? managerId, bool isRequired, string? currentManagerId, int? departmentId, IUnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(managerId))
            {
                if (isRequired)
                {
                    throw new AppException("Chưa chọn quản lý", 400);
                }
                return;
            }

            managerId = managerId.Trim();

            // Kiểm tra người dùng tồn tại và vai trò
            var user = await unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Id == managerId,
                includes: q => q.Include(u => u.Position)
            );

            if (user == null)
            {
                throw new AppException("Người dùng không tồn tại", 404);
            }

            var validManagerRoles = new[] { PositionNames.SeniorManager.ToLower(), PositionNames.MiddleManager.ToLower() };
            if (!validManagerRoles.Contains(user.Position?.PositionName?.ToLower()))
            {
                throw new AppException("Người dùng không phải là quản lý cấp cao hoặc cấp trung", 400);
            }

            // Kiểm tra ManagerId duy nhất
            if (managerId != currentManagerId)
            {
                var managerExists = await unitOfWork.DepartmentRepository.AnyAsync(
                    d => d.ManagerId == managerId && (!departmentId.HasValue || d.DepartmentId != departmentId.Value)
                );
                if (managerExists)
                {
                    throw new AppException("Quản lý đã được gán cho một phòng ban khác", 409);
                }
            }

            return;
        }

        public static List<string> GetPath(int departmentId, Dictionary<int, Department> departmentDict, Dictionary<int, List<string>> pathCache)
        {
            if (pathCache.TryGetValue(departmentId, out var cachedPath))
            {
                return cachedPath;
            }
            var path = new List<string>();
            var currentId = departmentId;

            while (currentId != 0)
            {
                if (!departmentDict.TryGetValue(currentId, out var currentDept))
                {
                    break;
                }

                path.Insert(0, currentDept.DepartmentName ?? "Unknown");
                currentId = currentDept.ParentId ?? 0;
            }

            pathCache[departmentId] = path;
            return path;
        }

        public static async Task<DepartmentDto> MapToDtoAsync(
            Department dept,
            Dictionary<int, Department> departmentDict,
            Dictionary<string, ApplicationUser> userDict,
            Dictionary<int, List<string>> pathCache,
            IMapper mapper)
        {
            var dto = mapper.Map<DepartmentDto>(dept);

            dto.DepartmentId = dept.DepartmentId;
            dto.ParentId = dept.ParentId;
            dto.ManagerId = dept.ManagerId;

            // Lấy ParentName
            if (dept.ParentId.HasValue && departmentDict.TryGetValue(dept.ParentId.Value, out var parent))
            {
                dto.ParentName = parent.DepartmentName;
            }

            // Lấy ManagerName
            if (!string.IsNullOrEmpty(dept.ManagerId) && userDict.TryGetValue(dept.ManagerId, out var manager))
            {
                dto.ManagerName = manager.FullName;
            }

            // Gán Level và Path
            dto.Level = dept.Level;
            dto.Path = GetPath(dept.DepartmentId, departmentDict, pathCache);

            // Gán thời gian
            dto.CreatedAt = dept.CreatedAt ?? DateTime.Now;
            dto.UpdatedAt = dept.UpdatedAt ?? DateTime.Now;

            // Xử lý Children nếu có
            if (dept.Children?.Any() == true)
            {
                dto.Children = (await Task.WhenAll(dept.Children.Select(
                    c => MapToDtoAsync(c, departmentDict, userDict, pathCache, mapper)
                ))).ToList();
            }
            else
            {
                dto.Children = new List<DepartmentDto>();
            }

            return dto;
        }
    }
}
