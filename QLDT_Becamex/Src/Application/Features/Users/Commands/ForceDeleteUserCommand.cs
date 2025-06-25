using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Users.Commands
{
    public record ForceDeleteUserCommand(string UserId) : IRequest<string>;
}
