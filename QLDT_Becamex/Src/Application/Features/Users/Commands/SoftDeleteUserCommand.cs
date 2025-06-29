using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Users.Commands
{
    public record SoftDeleteUserCommand(string UserId) : IRequest<string>;
}
