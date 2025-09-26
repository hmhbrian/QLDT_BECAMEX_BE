using AutoMapper;
using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Devices.Commands;
using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Infrastructure.Services;



namespace QLDT_Becamex.Src.Application.Features.Devices.Handlers
{
    public class CreateDevicesCommandHandler : IRequestHandler<CreateDevicesCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public CreateDevicesCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<string> Handle(CreateDevicesCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var (userId, _) = _userService.GetCurrentUserAuthenticationInfo();
            var devicesExists = await _unitOfWork.DevicesRepository.AnyAsync(d => d.DeviceToken == request.DeviceToken
                && d.UserId == userId);
            if (devicesExists)
                throw new AppException("User device này đã tồn tại", 409);

            var devices = _mapper.Map<Device>(request);

            devices.CreatedAt = DateTime.UtcNow;
            devices.UserId = userId;

            await _unitOfWork.DevicesRepository.AddAsync(devices);
            await _unitOfWork.CompleteAsync();

            return devices.Id.ToString();
        }
    }
}
