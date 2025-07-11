using MediatR;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Status.Commands
{
    public record CreateDepartmentStatusCommand(CreateStatusDto Request) : IRequest<Unit>;
    public record DeleteDepartmentStatusCommand(List<int> Ids) : IRequest<Unit>;
    public record UpdateDepartmentStatusCommand(int Id, CreateStatusDto Request) : IRequest<Unit>;
}
