using AutoMapper;
using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
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

        public async Task<Result<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            try
            {
                // Kiểm tra tên phòng ban đã tồn tại chưa
                var nameExists = await _unitOfWork.DepartmentRepository
                    .AnyAsync(d => d.DepartmentName == dto.Departmentname);
                if (nameExists)
                {
                    return Result<DepartmentDto>.Failure(
                        message: "Tạo phòng ban thất bại",
                        error: "Tên phòng ban đã tồn tại",
                        code: "DEPARTMENT_NAME_EXISTS",
                        statusCode: 400
                    );
                }

                var department = _mapper.Map<Department>(dto);
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                await _unitOfWork.CompleteAsync();

                var resultDto = _mapper.Map<DepartmentDto>(department);
                if (!string.IsNullOrEmpty(department.ParentId))
                {
                    var parent = await _unitOfWork.DepartmentRepository
                        .GetByIdAsync(department.ParentId);
                    resultDto.ParentName = parent?.DepartmentName;
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
        public async Task<Result<List<DepartmentDto>>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
                var departmentDtos = new List<DepartmentDto>();
                foreach (var dept in departments)
                {
                    var dto = _mapper.Map<DepartmentDto>(dept);
                    if (!string.IsNullOrEmpty(dept.ParentId))
                    {
                        var parent = await _unitOfWork.DepartmentRepository
                            .GetByIdAsync(dept.ParentId);
                        dto.ParentName = parent?.DepartmentName;
                    }
                    departmentDtos.Add(dto);
                }

                return Result<List<DepartmentDto>>.Success(
                    message: "Lấy danh sách phòng ban thành công",
                    code: "DEPARTMENTS_RETRIEVED",
                    statusCode: 200,
                    data: departmentDtos
                );
            }
            catch (Exception ex)
            {
                return Result<List<DepartmentDto>>.Failure(
                    error: ex.Message,
                    code: "SYSTEM_ERROR",
                    statusCode: 500
                );
            }
        }
    }
}
