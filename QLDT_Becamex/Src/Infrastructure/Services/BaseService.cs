using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Interfaces;
using System.Security.Claims;

namespace QLDT_Becamex.Src.Infrastructure.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        public BaseService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        public (string? UserId, string? Role) GetCurrentUserAuthenticationInfo()
        {
            var currentUser = _httpContextAccessor.HttpContext?.User;

            var userId = currentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = currentUser?.FindFirst(ClaimTypes.Role)?.Value;


            return (userId, role);
        }

        public async Task<List<int>> GetAllChildDepartmentIds(int parentDepartmentId)
        {
            try
            {
                var childDepartmentIds = new List<int>();
                // Sử dụng Repository để truy vấn trực tiếp các phòng ban con
                var directChildren = await _unitOfWork.DepartmentRepository
                                                            .FindAsync(d => d.ParentId == parentDepartmentId);

                foreach (var child in directChildren)
                {
                    childDepartmentIds.Add(child.DepartmentId);
                    // Đệ quy để lấy các phòng ban con của phòng ban con này
                    childDepartmentIds.AddRange(await GetAllChildDepartmentIds(child.DepartmentId));
                }
                return childDepartmentIds;
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần thiết
                Console.WriteLine($"Lỗi khi lấy ID phòng ban con: {ex.Message}");
                return new List<int>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public async Task ValidateManagerIdDeparmentAsync(string? managerId, bool isRequired, string? currentManagerId, int? departmentId)
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
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
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
                var managerExists = await _unitOfWork.DepartmentRepository.AnyAsync(
                    d => d.ManagerId == managerId && (!departmentId.HasValue || d.DepartmentId != departmentId.Value)
                );
                if (managerExists)
                {
                    throw new AppException("Quản lý đã được gán cho một phòng ban khác", 409);
                }
            }

            return;
        }
    }
}
