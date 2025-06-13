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
                int? parentId = request.ParentId == 0 ? null: request.ParentId;
                // Mặc định level = 1 nếu không có parent
                int calculatedLevel = 1;
                if (parentId != null)
                {
                    var parent = await _unitOfWork.DepartmentRepository.GetByIdAsync(parentId);
                    if(parent != null)
                        calculatedLevel = parent.level + 1;
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
                department.level = calculatedLevel; // Gán level được tính toán
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

        public List<string> GetPath(int? departmentId, List<Department> departments)
        {
            var path = new List<string>();
            var currentId = departmentId;

            while (currentId != null)
            {
                var currentDept = departments.FirstOrDefault(d => d.DepartmentId == currentId);
                if (currentDept == null) break;

                path.Insert(0, currentDept.DepartmentCode ?? "Unknown"); // Sử dụng DepartmentName cho Path
                currentId = currentDept.ParentId; // Tiếp tục với ParentId tiếp theo
            }

            return path;
        }


        public async Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
                var users = await _unitOfWork.UserRepository.GetAllAsync();

                var departmentDtos = await Task.WhenAll(departments.Select(async dept =>
                {
                    if (dept.DepartmentId < 0)
                    {
                        throw new InvalidOperationException($"DepartmentId is null for department: {dept.DepartmentName}");
                    }

                    var dto = _mapper.Map<DepartmentDto>(dept);
                    dto.DepartmentId = dept.DepartmentId;
                    dto.ParentId = dept.ParentId;
                    dto.ManagerId = dept.ManagerId;

                    if (dept.ParentId > 0)
                    {
                        var parent = departments.FirstOrDefault(d => d.DepartmentId == dept.ParentId);
                        dto.ParentName = parent?.DepartmentName;
                    }

                    if (!string.IsNullOrEmpty(dept.ManagerId))
                    {
                        var manager = users.FirstOrDefault(u => u.Id == dept.ManagerId);
                        dto.ManagerName = manager?.FullName;
                    }

                    dto.Level = dept.level;
                    dto.Path = GetPath(dept.DepartmentId, departments.ToList()); // Gọi đồng bộ
                    dto.CreatedAt = dept.CreatedAt ?? DateTime.Now;
                    dto.UpdatedAt = dept.UpdatedAt ?? DateTime.Now;
                    return dto;
                }).ToList());

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

    }
}
