using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Queries
{
    public record GetListUsersQuery(BaseQueryParam BaseQueryParam) : IRequest<PagedResult<UserDto>>;
}
