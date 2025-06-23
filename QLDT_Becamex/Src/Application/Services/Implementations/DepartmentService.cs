using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Dtos;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Infrastructure.Persistence.UnitOfWork;
using QLDT_Becamex.Src.Services.Interfaces;


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
                var nameExists = await _unitOfWork.DepartmentRepository.AnyAsync(d => d.DepartmentName == request.DepartmentName || d.DepartmentCode == request.DepartmentCode);
                if (nameExists)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Tạo phòng ban thất bại",
                        error: "Tên phòng ban hoặc mã phòng ban đã tồn tại",
                        code: "EXISTS",
                        statusCode: 409
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
                string? managerId = request.ManagerId.Trim();

                var managerCheckResult = await ValidateManagerIdAsync(
                    managerId: request.ManagerId,
                    isRequired: true, // Bắt buộc ManagerId
                    currentManagerId: null, // Không có quản lý hiện tại
                    departmentId: null // Không có DepartmentId khi tạo mới
                );

                if (!managerCheckResult.IsSuccess)
                {
                    return Result<DepartmentDto>.Failure(
                        message: managerCheckResult.Message,
                        error: managerCheckResult.Errors.First(),
                        code: managerCheckResult.Code,
                        statusCode: managerCheckResult.StatusCode
                    );
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
                    code: "SUCCESS",
                    statusCode: 200,
                    data: resultDto
                );
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(
                    error: "Lỗi hệ thống: " + ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }

        private async Task<Result<bool>> ValidateManagerIdAsync(string? managerId, bool isRequired, string? currentManagerId, int? departmentId)
        {
            if (string.IsNullOrWhiteSpace(managerId))
            {
                if (isRequired)
                {
                    return Result<bool>.Failure(
                        message: isRequired ? "Tạo phòng ban thất bại" : "Cập nhật phòng ban thất bại",
                        error: "Chưa chọn quản lý",
                        code: "INVALID",
                        statusCode: 400
                    );
                }
                return Result<bool>.Success(data: true);
            }

            managerId = managerId.Trim();

            // Kiểm tra người dùng tồn tại và vai trò
            var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(
                u => u.Id == managerId,
                includes: q => q.Include(u => u.Position)
            );

            if (user == null)
            {
                return Result<bool>.Failure(
                    message: isRequired ? "Tạo phòng ban thất bại" : "Cập nhật phòng ban thất bại",
                    error: "Người dùng không tồn tại",
                    code: "NOT_FOUND",
                    statusCode: 404
                );
            }

            var validManagerRoles = new[] { PositionNames.SeniorManager.ToLower(), PositionNames.MiddleManager.ToLower() };
            if (!validManagerRoles.Contains(user.Position?.PositionName?.ToLower()))
            {
                return Result<bool>.Failure(
                    message: isRequired ? "Tạo phòng ban thất bại" : "Cập nhật phòng ban thất bại",
                    error: "Người dùng không phải là quản lý cấp cao hoặc cấp trung",
                    code: "INVALID",
                    statusCode: 400
                );
            }

            // Kiểm tra ManagerId duy nhất
            if (managerId != currentManagerId)
            {
                var managerExists = await _unitOfWork.DepartmentRepository.AnyAsync(
                    d => d.ManagerId == managerId && (!departmentId.HasValue || d.DepartmentId != departmentId.Value)
                );
                if (managerExists)
                {
                    return Result<bool>.Failure(
                        message: isRequired ? "Tạo phòng ban thất bại" : "Cập nhật phòng ban thất bại",
                        error: "Quản lý đã được gán cho một phòng ban khác",
                        code: "EXISTS",
                        statusCode: 409
                    );
                }
            }

            return Result<bool>.Success(data: true);
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

                path.Insert(0, currentDept.DepartmentName ?? "Unknown");
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
                    includes: q => q
                        .Include(d => d.Parent)
                        .Include(d => d.Manager)
                        .Include(d => d.Children)
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
                    code: "SUCCESS",
                    statusCode: 200,
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
                    includes: q => q
                    .Include(d => d.Parent)
                    .Include(d => d.Manager)
                    .Include(d => d.Children)
                );

                if (!department.Any())
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Không tìm thấy phòng ban",
                        error: "Phòng ban không tồn tại",
                        code: "NOT_FOUND",
                        statusCode: 404
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
                    code: "SUCCESS",
                    statusCode: 200,
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
                     includes: q => q
                    .Include(d => d.Parent)
                    .Include(d => d.Manager)
                    .Include(d => d.Children)
                );

                if (!departments.Any())
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Cập nhật phòng ban thất bại",
                        error: "Phòng ban không tồn tại",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }

                var department = departments.First();
                if (request.DepartmentName == null || request.DepartmentCode == null)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Cập nhật phòng ban thất bại",
                        error: "Tên hoặc mã phòng ban không được để trống",
                        code: "INVALID",
                        statusCode: 400
                    );
                }

                // Kiểm tra tên phòng ban đã tồn tại chưa (ngoại trừ chính nó)
                var nameExists = await _unitOfWork.DepartmentRepository
                    .AnyAsync(d => d.DepartmentName == request.DepartmentName || d.DepartmentCode == request.DepartmentCode && d.DepartmentId != id);
                if (nameExists)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Cập nhật phòng ban thất bại",
                        error: "Tên phòng ban hoặc mã phòng ban đã tồn tại",
                        code: "EXISTS",
                        statusCode: 409
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
                            code: "NOT_FOUND",
                            statusCode: 404
                        );
                    }

                    // Kiểm tra vòng lặp
                    if (HasCycle(id, newParentId, departmentDict))
                    {
                        return Result<DepartmentDto>.Failure(
                            message: "Cập nhật phòng ban thất bại",
                            error: "Thay đổi ParentId tạo ra vòng lặp trong cây phân cấp",
                            code: "INVALID",
                            statusCode: 400
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
                string? managerId = request.ManagerId;

                var managerCheckResult = await ValidateManagerIdAsync(
                    managerId: managerId,
                    isRequired: false, // ManagerId không bắt buộc khi cập nhật
                    currentManagerId: department.ManagerId,
                    departmentId: id
                );

                if (!managerCheckResult.IsSuccess)
                {
                    return Result<DepartmentDto>.Failure(
                        message: managerCheckResult.Message,
                        error: managerCheckResult.Errors.First(),
                        code: managerCheckResult.Code,
                        statusCode: managerCheckResult.StatusCode
                    );
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
                    code: "SUCCESS",
                    statusCode: 200,
                    data: resultDto
                );
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure(
                    message: "Cập nhật phòng ban thất bại do lỗi hệ thống" + ex.Message,
                    error: "Vui lòng thử lại sau",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
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

        public async Task<Result<bool>> DeleteDepartmentAsync(int id)
        {
            try
            {
                // Lấy phòng ban theo ID 
                var departments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: false,
                    includes: q => q
                        .Include(d => d.Children)
                        .Include(d => d.Parent)
                        .Include(d => d.Manager)
                );
                if (!departments.Any())
                {
                    return Result<bool>.Failure(
                        message: "Xóa phòng ban thất bại",
                        error: "Phòng ban không tồn tại",
                        code: "NOT_FOUND",
                        statusCode: 404
                    );
                }
                var department = departments.First();

                // Lấy tất cả phòng ban
                var allDepartments = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: null,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true
                );

                var departmentDict = allDepartments.ToDictionary(d => d.DepartmentId, d => d);

                // Cập nhật DepartmentId của các user liên kết thành null
                var usersInDepartment = await _unitOfWork.UserRepository.GetFlexibleAsync(
                    predicate: u => u.DepartmentId == id,
                    asNoTracking: false // Cần theo dõi để cập nhật
                );

                foreach (var user in usersInDepartment)
                {
                    user.DepartmentId = null;
                    _unitOfWork.UserRepository.Update(user);
                }

                // Xử lý phòng ban con
                if (department.Children?.Any() == true)
                {
                    int newLevel;
                    int? newParentId;

                    if (department.Level == 1)
                    {
                        // Phòng ban cấp 1: con trở thành cấp 1, ParentId = null
                        newLevel = 1;
                        newParentId = null;
                    }
                    else
                    {
                        // Phòng ban không phải cấp 1: con kế thừa ParentId của phòng ban hiện tại
                        newLevel = department.ParentId.HasValue ? department.Level : 1;
                        newParentId = department.ParentId;
                    }

                    // Cập nhật các phòng ban con
                    await UpdateChildrenAfterDeleteAsync(department, newParentId, newLevel, departmentDict);
                }

                // Xóa phòng ban
                _unitOfWork.DepartmentRepository.Remove(department);
                await _unitOfWork.CompleteAsync();

                return Result<bool
                    >.Success(
                    message: "Xóa phòng ban thành công",
                    code: "SUCCESS",
                    statusCode: 200,
                    data: true
                );

            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(
                    message: "Xóa phòng ban thất bại do lỗi hệ thống" + ex.Message,
                    error: "Vui lòng thử lại sau",
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }

        }

        //Cập nhật phòng ban con sau khi xóa phòng ban cha
        private async Task UpdateChildrenAfterDeleteAsync(
            Department parent,
            int? newParentId,
            int newLevel,
            Dictionary<int, Department> departmentDict)
        {
            if (parent.Children == null || !parent.Children.Any())
            {
                return;
            }
            foreach (var child in parent.Children)
            {
                child.ParentId = newParentId;
                child.Level = newLevel;
                child.Status = "inactive";
                child.UpdatedAt = DateTime.Now;

                departmentDict[child.DepartmentId] = child;

                // Đệ quy cập nhật các phòng ban con
                await UpdateChildrenAfterDeleteAsync(child, newParentId, newLevel + 1, departmentDict);
            }
        }
    }
}
