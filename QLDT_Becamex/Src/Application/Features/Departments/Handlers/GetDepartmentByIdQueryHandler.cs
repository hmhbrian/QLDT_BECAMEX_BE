using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Application.Features.Departments.Queries;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services.DepartmentServices;

namespace QLDT_Becamex.Src.Application.Features.Departments.Handlers
{
    public class GetDepartmentByIdQueryHandler : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public GetDepartmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IDepartmentService departmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _departmentService = departmentService;
        }

        public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Tải phòng ban theo ID 
                var department = await _unitOfWork.DepartmentRepository.GetFlexibleAsync(
                    predicate: d => d.DepartmentId == request.Id,
                    orderBy: null,
                    page: null,
                    pageSize: null,
                    asNoTracking: true,
                    includes: q => q
                    .Include(d => d.Parent)
                    .Include(d => d.Manager)
                    .Include(d => d.Children)
                    .Include(d => d.Status)
                );

                if (!department.Any())
                {
                    throw new AppException("Không tìm thấy phòng ban", 404);
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
                var departmentDto = await _departmentService.MapToDtoAsync(dept, departmentDict, userDict, pathCache, _mapper);

                return departmentDto;
            }
            catch (Exception)
            {
                throw new AppException("Vui lòng thử lại sau", 500);
            }
        }
    }
}
