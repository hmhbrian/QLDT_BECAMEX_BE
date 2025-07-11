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
    public class GetAllDepartmentQueryHandler : IRequestHandler<GetAllDepartmentQuery, List<DepartmentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public GetAllDepartmentQueryHandler(IMapper mapper, IUnitOfWork unitOfWork, IDepartmentService departmentService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _departmentService = departmentService;
        }

        public async Task<List<DepartmentDto>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Lấy tất cả phòng ban với các liên kết cần thiết
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
                        .Include(d => d.Status)
                );

                // Lấy tất cả người dùng để ánh xạ ManagerName
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

                // Ánh xạ danh sách Department sang DepartmentDto
                var departmentDtos = await Task.WhenAll(allDepartments.Select(
                    dept => _departmentService.MapToDtoAsync(dept, departmentDict, userDict, pathCache, _mapper)));

                return departmentDtos.ToList();
            }
            catch (Exception)
            {
                throw new AppException("Vui lòng thử lại sau", 500);
            }
        }
    }
}
