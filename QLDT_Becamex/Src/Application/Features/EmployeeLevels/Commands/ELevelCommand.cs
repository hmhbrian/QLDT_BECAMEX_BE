using MediatR;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.EmployeeLevels.Commands
{
    public record CreateELevelCommand(CreateELevelDto Request) : IRequest<string>;
    public record UpdateELevelCommand(int Id, CreateELevelDto Request) : IRequest<string>;
    public record DeleteELevelCommand(int Id) : IRequest<string>;

}
