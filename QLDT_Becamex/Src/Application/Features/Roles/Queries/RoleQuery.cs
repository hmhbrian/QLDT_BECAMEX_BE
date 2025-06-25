using MediatR;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Roles.Queries
{
    public record GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>;
}
