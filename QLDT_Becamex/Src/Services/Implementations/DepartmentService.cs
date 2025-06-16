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

                // Hàm ánh xạ Department sang DepartmentDto
                async Task<DepartmentDto> MapToDtoAsync(Department dept)
                {
                    var dto = _mapper.Map<DepartmentDto>(dept);

                    // Gán các thuộc tính
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

                    dto.Level = dept.Level;
                    dto.Path = GetPath(dept.DepartmentId, departmentDict, pathCache);
                    dto.CreatedAt = dept.CreatedAt ?? DateTime.Now;
                    dto.UpdatedAt = dept.UpdatedAt ?? DateTime.Now;

                    // Xử lý Children nếu có
                    if (dept.Children?.Any() == true)
                    {
                        dto.Children = (await Task.WhenAll(dept.Children.Select(MapToDtoAsync))).ToList();
                    }
                    else
                    {
                        dto.Children = new List<DepartmentDto>();
                    }

                    return dto;
                }

                var departmentDtos = await Task.WhenAll(allDepartments.Select(MapToDtoAsync));

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

                // Hàm ánh xạ Department sang DepartmentDto
                async Task<DepartmentDto> MapToDtoAsync(Department dept)
                {
                    var dto = _mapper.Map<DepartmentDto>(dept);

                    // Gán các thuộc tính
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
                        dto.Children = (await Task.WhenAll(dept.Children.Select(MapToDtoAsync))).ToList();
                    }
                    else
                    {
                        dto.Children = new List<DepartmentDto>();
                    }

                    return dto;
                }

                // Ánh xạ phòng ban sang DTO
                var departmentDto = await MapToDtoAsync(dept);

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
    }
}
