using MediatR;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;

namespace QLDT_Becamex.Src.Application.Features.EmployeeLevels.Queries
{
    public record GetAllELevelsQuery : IRequest<IEnumerable<ELevelDto>>;
}