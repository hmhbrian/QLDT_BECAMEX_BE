using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Commands;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.Departments.Helpers;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Application.Features.Departments.Handlers
{
    public class GetDepartmentByIdCommandHandler : IRequestHandler<GetDepartmentByIdCommand, DepartmentDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetDepartmentByIdCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DepartmentDto> Handle(GetDepartmentByIdCommand request, CancellationToken cancellationToken)
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
                var departmentDto = await DepartmentHelper.MapToDtoAsync(dept, departmentDict, userDict, pathCache, _mapper);

                return departmentDto;
            }
            catch (Exception)
            {
                throw new AppException("Vui lòng thử lại sau", 500);
            }
        }
    }
}
