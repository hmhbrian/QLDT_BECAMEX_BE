using Microsoft.AspNetCore.Http;
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

    }
}
