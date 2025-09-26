using MediatR;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Devices.Commands
{
    public record CreateDevicesCommand(CreateDeviceDto Request) : IRequest<string>;
}
