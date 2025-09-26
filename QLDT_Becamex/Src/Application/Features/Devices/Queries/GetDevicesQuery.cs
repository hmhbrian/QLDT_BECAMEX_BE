using MediatR;
using QLDT_Becamex.Src.Application.Features.Devices.Dtos;
namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Queries
{
    public record GetDevicesQuery() : IRequest<List<DeviceDto>>;
}