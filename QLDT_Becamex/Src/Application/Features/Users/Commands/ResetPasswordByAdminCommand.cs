using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Commands
{
    public record ResetPasswordByAdminCommand(string UserId, UserResetPasswordDto Request) : IRequest<string>;
}
