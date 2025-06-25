using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Departments.Commands
{
    public record DeleteDepartmentCommand(int Id) : IRequest<bool>;
}
