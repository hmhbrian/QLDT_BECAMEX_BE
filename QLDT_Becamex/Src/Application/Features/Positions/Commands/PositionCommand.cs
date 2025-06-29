using MediatR;
using QLDT_Becamex.Src.Application.Features.Positions.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Positions.Commands
{
    public record CreatePositionCommand(CreatePositionDto Request) : IRequest<string>;
    public record UpdatePositionCommand(int Id, CreatePositionDto Request) : IRequest<string>;
    public record DeletePositionCommand(int Id) : IRequest<string>;

}
