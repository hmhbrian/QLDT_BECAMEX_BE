using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Queries
{
    public record GetUserByIdQuery(string UserId) : IRequest<UserDto>;
}
