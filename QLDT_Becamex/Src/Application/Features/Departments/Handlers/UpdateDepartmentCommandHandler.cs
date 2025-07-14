using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;


namespace QLDT_Becamex.Src.Application.Features.Departments.Handlers
{
    public class UpdateDepartmentCommandHandler : IRequestHandler<UpdateDepartmentCommand, DepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public UpdateDepartmentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IDepartmentService departmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _departmentService = departmentService;
        }

        public async Task<DepartmentDto> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var id = command.Id;
                var request = command.Request;

                // Kiểm tra xem phòng ban có tồn tại không
                var departments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: false, // Cần theo dõi để cập nhật
                    includes: q => q
                        .Include(d => d.Parent)
                        .Include(d => d.Manager)
                        .Include(d => d.Children)
                );

                if (!departments.Any())
                {
                    throw new AppException("Phòng ban không tồn tại", 404);
                }

                var department = departments.First();

                // Kiểm tra tên hoặc mã phòng ban không được để trống
                if (string.IsNullOrWhiteSpace(request.DepartmentName) || string.IsNullOrWhiteSpace(request.DepartmentCode))
                {
                    throw new AppException("Tên hoặc mã phòng ban không được để trống", 400);
                }

                // Kiểm tra tên hoặc mã phòng ban đã tồn tại (ngoại trừ chính nó)
                if (request.DepartmentName != department.DepartmentName || request.DepartmentCode != department.DepartmentCode)
                {
                    var nameExists = await _unitOfWork.DepartmentRepository.AnyAsync(
                        d => (d.DepartmentName == request.DepartmentName || d.DepartmentCode == request.DepartmentCode) && d.DepartmentId != id
                    );
                    if (nameExists)
                    {
                        throw new AppException("Tên phòng ban hoặc mã phòng ban đã tồn tại", 409);
                    }
                }

                if (request.StatusId.HasValue && !await _unitOfWork.DepartmentStatusRepository.AnyAsync(s => s.Id == request.StatusId.Value))
                    throw new AppException("Trạng thái phòng ban không hợp lệ", 400);

                // Lấy tất cả phòng ban để tra cứu và kiểm tra vòng lặp
                var allDepartments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                var departmentDict = allDepartments.ToDictionary(d => d.DepartmentId, d => d);

                // Xử lý ParentId và Level
                int? newParentId = request.ParentId == 0 ? null : request.ParentId;
                int newLevel = 1;
                Department? parent = null;
                if (newParentId != null)
                {
                    if (!departmentDict.TryGetValue(newParentId.Value, out parent))
                    {
                        throw new AppException("Phòng ban cha không tồn tại", 404);
                    }

                    // Kiểm tra vòng lặp
                    if (HasCycle(id, newParentId, departmentDict))
                    {
                        throw new AppException("Thay đổi ParentId tạo ra vòng lặp trong cây phân cấp", 400);
                    }

                    newLevel = parent.Level + 1;
                }
                else if (department.ParentId != null)
                {
                    newLevel = 1; // Nếu xóa parentId, trở thành phòng ban gốc
                }
                else
                {
                    newLevel = department.Level; // Giữ nguyên level nếu không thay đổi parent
                }

                // Tính chênh lệch level
                int levelDifference = newLevel - department.Level;

                // Kiểm tra và cập nhật ManagerId
                var managerId = request.ManagerId?.Trim();
                await _departmentService.ValidateManagerIdDeparmentAsync(managerId, false, department.ManagerId, id);

                // Cập nhật thông tin phòng ban
                var updatedDepartment = new Department
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = request.DepartmentName.Trim(),
                    DepartmentCode = request.DepartmentCode.Trim().ToLower(),
                    Description = request.Description?.Trim(),
                    ParentId = newParentId,
                    ManagerId = managerId,
                    StatusId = request.StatusId,
                    Level = newLevel,
                    UpdatedAt = DateTime.Now
                };

                // Cập nhật departmentDict với thông tin mới
                departmentDict[department.DepartmentId] = updatedDepartment;

                // Cập nhật level của các phòng ban con
                if (levelDifference != 0 && department.Children?.Any() == true)
                {
                    await UpdateChildrenLevels(department, levelDifference, departmentDict);
                }

                _unitOfWork.DepartmentRepository.Update(department, updatedDepartment);
                await _unitOfWork.CompleteAsync();

                // Lấy tất cả người dùng để ánh xạ ManagerName
                var allUsers = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );
                var userDict = allUsers.ToDictionary(u => u.Id, u => u);

                // Cache cho GetPath
                var pathCache = new Dictionary<int, List<string>>();

                // Ánh xạ sang DepartmentDto
                var resultDto = await _departmentService.MapToDtoAsync(department, departmentDict, userDict, pathCache, _mapper);

                return resultDto;
            }
            catch (Exception)
            {
                throw new AppException("Cập nhật phòng ban thất bại do lỗi hệ thống", 500);
            }
        }

        private bool HasCycle(int currentId, int? newParentId, Dictionary<int, Department> departmentDict)
        {
            var visited = new HashSet<int>();
            var parentId = newParentId;

            while (parentId != null)
            {
                if (visited.Contains(parentId.Value))
                {
                    return true; // Phát hiện vòng lặp
                }

                if (parentId == currentId)
                {
                    return true; // Tự tham chiếu
                }

                visited.Add(parentId.Value);
                if (!departmentDict.TryGetValue(parentId.Value, out var parent))
                {
                    return false;
                }

                parentId = parent.ParentId;
            }

            return false;
        }

        private async Task UpdateChildrenLevels(Department parent, int levelDifference, Dictionary<int, Department> departmentDict)
        {
            if (levelDifference == 0 || parent.Children == null || !parent.Children.Any())
            {
                return;
            }

            foreach (var child in parent.Children)
            {
                child.Level += levelDifference;
                departmentDict[child.DepartmentId] = child;
                await UpdateChildrenLevels(child, levelDifference, departmentDict); // Đệ quy
            }
        }
    }
}
