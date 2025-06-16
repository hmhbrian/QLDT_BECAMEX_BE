using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Dtos.Departments;
using QLDT_Becamex.Src.Dtos.Results;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Services.Interfaces;
using QLDT_Becamex.Src.UnitOfWork;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<DepartmentDto>> CreateDepartmentAsync(DepartmentRq request)
        {
            try
            {
                // Kiểm tra tên phòng ban đã tồn tại chưa
                var nameExists = await _unitOfWork.DepartmentRepository.AnyAsync(d => d.DepartmentName == request.DepartmentName);
                if (nameExists)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Tạo phòng ban thất bại",
                        error: "Tên phòng ban đã tồn tại",
                        code: "DEPARTMENT_NAME_EXISTS",
                        statusCode: 400
                    );
                }

                // Kiểm tra và xử lý ParentId
                int? parentId = request.ParentId == 0 ? null : request.ParentId;
                // Mặc định level = 1 nếu không có parent
                int calculatedLevel = 1;
                if (parentId != null)
                {
                    var parent = await _unitOfWork.DepartmentRepository.GetByIdAsync(parentId);
                    if (parent != null)
                        calculatedLevel = parent.Level + 1;
                }


                // Kiểm tra ManagerId duy nhất
                string? managerId = request.ManagerId?.ToLower() == "null" ? null : request.ManagerId;
                if (!string.IsNullOrEmpty(managerId))
                {
                    var existingMag = await _unitOfWork.DepartmentRepository
                        .AnyAsync(d => d.ManagerId == managerId);
                    if (existingMag)
                    {
                        return Result<DepartmentDto>.Failure(
                            message: "Tạo phòng ban thất bại",
                            error: "Manager đã được gán cho một phòng ban khác",
                            code: "MANAGER_ALREADY_ASSIGNED",
                            statusCode: StatusCodes.Status400BadRequest
                        );
                    }
                }

                // Ánh xạ dữ liệu
                var department = _mapper.Map<Department>(request);
                department.ParentId = parentId;
                department.ManagerId = managerId;
                department.Level = calculatedLevel; // Gán level được tính toán
                department.CreatedAt = DateTime.Now;
                department.UpdatedAt = DateTime.Now;

                await _unitOfWork.DepartmentRepository.AddAsync(department);
                await _unitOfWork.CompleteAsync();

                var resultDto = _mapper.Map<DepartmentDto>(department);
                resultDto.DepartmentId = department.DepartmentId;

                if (department.ParentId != null)
                {
                    var parent = await _unitOfWork.DepartmentRepository
                        .GetByIdAsync(department.ParentId);
                    resultDto.ParentName = parent?.DepartmentName;
                }

                if (!string.IsNullOrEmpty(department.ManagerId))
                {
                    var manager = await _unitOfWork.UserRepository
                        .GetByIdAsync(department.ManagerId);
                    resultDto.ManagerName = manager?.FullName;
                }

                return Result<DepartmentDto>.Success(
                    message: "Tạo phòng ban thành công",
                    code: "DEPARTMENT_CREATED",
                    statusCode: 201,
                    data: resultDto
                );
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        public List<string> GetPath(int departmentId, Dictionary<int, Department> departmentDict, Dictionary<int, List<string>> pathCache)
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

                path.Insert(0, currentDept.DepartmentCode ?? "Unknown");
                currentId = currentDept.ParentId ?? 0;
            }

            pathCache[departmentId] = path;
            return path;
        }

        private async Task<DepartmentDto> MapToDtoAsync(
            Department dept,
            Dictionary<int, Department> departmentDict,
            Dictionary<string, ApplicationUser> userDict,
            Dictionary<int, List<string>> pathCache)
        {
            var dto = _mapper.Map<DepartmentDto>(dept);

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
                    c => MapToDtoAsync(c, departmentDict, userDict, pathCache)
                ))).ToList();
            }
            else
            {
                dto.Children = new List<DepartmentDto>();
            }

            return dto;
        }

        public async Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            try
            {
                var allDepartments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                     predicate: null,
                     orderBy: null,
                     page: null,
                     pageSize: null,
                     asNoTracking: true,
                     includes: new Expression<Func<Department, object>>[]
                     { 
                        d => d.Parent,
                        d => d.Manager,
                        d => d.Children
                     }
                 );

                // Tải tất cả người dùng
                var allUsers = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                // Tạo Dictionary để tra cứu nhanh
                var departmentDict = allDepartments.ToDictionary(d => d.DepartmentId, d => d);
                var userDict = allUsers.ToDictionary(u => u.Id, u => u);

                // Cache cho GetPath
                var pathCache = new Dictionary<int, List<string>>();

                // Ánh xạ Department sang DTO
                var departmentDtos = await Task.WhenAll(allDepartments.Select(
                    dept => MapToDtoAsync(dept, departmentDict, userDict, pathCache)));

                return Result<List<DepartmentDto>>.Success(
                    message: "Lấy danh sách phòng ban thành công",
                    code: "DEPARTMENTS_RETRIEVED",
                    statusCode: StatusCodes.Status200OK,
                    data: departmentDtos.ToList()
                );
            }
            catch (Exception ex)
            {
                return Result<List<DepartmentDto>>.Failure(
                    message: ex.Message,
                    error: "Vui lòng thử lại sau",
                    code: "SYSTEM_ERROR",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<Result<DepartmentDto>> GetDepartmentByIdAsync(int id)
        {
            try
            {
                // Tải phòng ban theo ID 
                var department = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true,
                    includes: new Expression<Func<Department, object>>[]
                    {
                        d => d.Parent,
                        d => d.Manager,
                        d => d.Children
                    }
                );

                if (!department.Any())
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Không tìm thấy phòng ban",
                        error: "Phòng ban không tồn tại",
                        code: "DEPARTMENT_NOT_FOUND",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }

                var dept = department.First();

                // Tải tất cả phòng ban để tra cứu ParentName
                var allDepartments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                // Tải tất cả người dùng để tra cứu ManagerName
                var allUsers = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                // Tạo Dictionary để tra cứu nhanh
                var departmentDict = allDepartments.ToDictionary(d => d.DepartmentId, d => d);
                var userDict = allUsers.ToDictionary(u => u.Id, u => u);

                // Cache cho GetPath
                var pathCache = new Dictionary<int, List<string>>();

                // ánh xạ Department sang DepartmentDto
                var departmentDto = await MapToDtoAsync(dept, departmentDict, userDict, pathCache);

                return Result<DepartmentDto>.Success(
                    message: "Lấy thông tin phòng ban thành công",
                    code: "DEPARTMENT_RETRIEVED",
                    statusCode: StatusCodes.Status200OK,
                    data: departmentDto
                );
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(
                    message: ex.Message,
                    error: "Vui lòng thử lại sau",
                    code: "SYSTEM_ERROR",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }


        public async Task<Result<DepartmentDto>> UpdateDepartmentAsync(int id, DepartmentRq request)
        {
            try
            {
                // Tải phòng ban theo ID với các liên kết cần thiết
                var departments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: false,
                    includes: new Expression<Func<Department, object>>[]
                    {
                        d => d.Parent,
                        d => d.Manager,
                        d => d.Children
                    }
                );

                if (!departments.Any())
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Cập nhật phòng ban thất bại",
                        error: "Phòng ban không tồn tại",
                        code: "DEPARTMENT_NOT_FOUND",
                        statusCode: StatusCodes.Status404NotFound
                    );
                }

                var department = departments.First();

                // Kiểm tra tên phòng ban đã tồn tại chưa (ngoại trừ chính nó)
                var nameExists = await _unitOfWork.DepartmentRepository
                    .AnyAsync(d => d.DepartmentName == request.DepartmentName && d.DepartmentId != id);
                if (nameExists)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Cập nhật phòng ban thất bại",
                        error: "Tên phòng ban đã tồn tại",
                        code: "DEPARTMENT_NAME_EXISTS",
                        statusCode: StatusCodes.Status400BadRequest
                    );
                }

                // Tải tất cả phòng ban để tra cứu và kiểm tra vòng lặp
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
                        return Result<DepartmentDto>.Failure(
                            message: "Cập nhật phòng ban thất bại",
                            error: "Phòng ban cha không tồn tại",
                            code: "PARENT_NOT_FOUND",
                            statusCode: StatusCodes.Status400BadRequest
                        );
                    }

                    // Kiểm tra vòng lặp
                    if (HasCycle(id, newParentId, departmentDict))
                    {
                        return Result<DepartmentDto>.Failure(
                            message: "Cập nhật phòng ban thất bại",
                            error: "Thay đổi ParentId tạo ra vòng lặp trong cây phân cấp",
                            code: "CYCLE_DETECTED",
                            statusCode: StatusCodes.Status400BadRequest
                        );
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

                // Kiểm tra ManagerId duy nhất (ngoại trừ chính nó)
                string? managerId = request.ManagerId?.ToLower() == "null" ? null : request.ManagerId;
                if (!string.IsNullOrEmpty(managerId) && managerId != department.ManagerId)
                {
                    var managerExists = await _unitOfWork.DepartmentRepository.AnyAsync(
                d => d.ManagerId == managerId && d.DepartmentId != id
            );
                    if (managerExists)
                    {
                        return Result<DepartmentDto>.Failure(
                            message: "Cập nhật phòng ban thất bại",
                            error: "Manager đã được gán cho một phòng ban khác",
                            code: "MANAGER_ALREADY_ASSIGNED",
                            statusCode: StatusCodes.Status400BadRequest
                        );
                    }
                }

                // Cập nhật thông tin
                department.DepartmentName = request.DepartmentName;
                department.DepartmentCode = request.DepartmentCode;
                department.Description = request.Description;
                department.ParentId = newParentId;
                department.ManagerId = managerId;
                department.Status = request.Status;
                department.Level = newLevel;
                department.UpdatedAt = DateTime.Now;

                // Cập nhật departmentDict với thông tin mới của phòng ban
                departmentDict[department.DepartmentId] = department;

                // Cập nhật level của các phòng ban con
                await UpdateChildrenLevels(department, levelDifference, departmentDict);

                _unitOfWork.DepartmentRepository.Update(department);
                await _unitOfWork.CompleteAsync();

                // Tải user để lấy ManagerName
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

                // Ánh xạ DTO
                var resultDto = await MapToDtoAsync(department, departmentDict, userDict, pathCache);

                return Result<DepartmentDto>.Success(
                    message: "Cập nhật phòng ban thành công",
                    code: "DEPARTMENT_UPDATED",
                    statusCode: StatusCodes.Status200OK,
                    data: resultDto
                );
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(
                    message: "Cập nhật phòng ban thất bại do lỗi hệ thống" + ex.Message,
                    error: "Vui lòng thử lại sau",
                    code: "SYSTEM_ERROR",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
        
        // Kiểm tra vòng lặp
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

        
        // Cập nhật level của các phòng ban con
        private async Task UpdateChildrenLevels(
            Department parent,
            int levelDifference,
            Dictionary<int, Department> departmentDict)
        {
            if (levelDifference == 0)
            {
                return;
            }

            // Sử dụng Children đã tải từ truy vấn ban đầu
            if (parent.Children == null || !parent.Children.Any())
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
