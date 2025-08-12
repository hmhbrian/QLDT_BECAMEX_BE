using AutoMapper;
using MediatR;
using Microsoft.AspNet.Identity;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Queries;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;

namespace QLDT_Becamex.Src.Application.Features.Users.Handlers
{
    public class GetManagerForDeptQueryHandler : IRequestHandler<GetManagerForDeptQuery, List<ManagerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetManagerForDeptQueryHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<List<ManagerDto>> Handle(GetManagerForDeptQuery request, CancellationToken cancellationToken)
        { 
            //Lấy tất cả managerId trong Department
            var departmentUserIds = (await _unitOfWork.DepartmentRepository.GetFlexibleAsync(asNoTracking: true)).Select(d => d.ManagerId).ToHashSet();

            //Lấy user là Manager và chưa có trong department
            var users = (await _unitOfWork.UserRepository.GetFlexibleAsync(
                predicate: u => u.ELevelId >= 4 && u.IsDeleted == false && !departmentUserIds.Contains(u.Id),
                asNoTracking: true
            )).ToList();

            var managerDtos = _mapper.Map<List<ManagerDto>>(users);
            return managerDtos;
        }
    }
}
