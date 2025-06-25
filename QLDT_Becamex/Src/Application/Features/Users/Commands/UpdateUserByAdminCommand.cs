
using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Commands
{
    public record UpdateUserByAdminCommand(string UserId, UserAdminUpdateDto Request) : IRequest<string>;
}
