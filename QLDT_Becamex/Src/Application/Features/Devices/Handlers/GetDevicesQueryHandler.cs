using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Queries;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;
using QLDT_Becamex.Src.Infrastructure.Services;
using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Entities;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using QLDT_Becamex.Src.Application.Features.Devices.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Handlers
{
    public class GetDevicesQueryHandler : IRequestHandler<GetDevicesQuery, List<DeviceDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public GetDevicesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<List<DeviceDto>> Handle(GetDevicesQuery request, CancellationToken cancellationToken)
        {
            var devices = await _unitOfWork.DevicesRepository.GetAllAsync();
            var dto = _mapper.Map<List<DeviceDto>>(devices);
            return dto;
        }
    }
}