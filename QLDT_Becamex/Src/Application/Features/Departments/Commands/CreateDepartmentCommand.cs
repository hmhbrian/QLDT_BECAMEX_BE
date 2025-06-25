using MediatR;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Departments.Commands
{
    public record CreateDepartmentCommand(DepartmentRequestDto Request) : IRequest<string>;
}
