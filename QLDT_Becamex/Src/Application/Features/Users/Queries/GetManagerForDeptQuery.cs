using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Queries
{
    public record GetManagerForDeptQuery : IRequest<List<ManagerDto>>;
}
