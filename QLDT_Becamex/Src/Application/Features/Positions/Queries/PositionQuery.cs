using MediatR;
using QLDT_Becamex.Src.Application.Features.Positions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Positions.Queries
{
    public record GetAllPositionsQuery : IRequest<IEnumerable<PositionDto>>;
}