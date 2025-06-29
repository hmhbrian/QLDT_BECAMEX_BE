using MediatR;
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Departments.Queries
{
    public record GetAllDepartmentQuery : IRequest<List<DepartmentDto>>;
}
