using MediatR;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Users.Commands
{
    public record CreateUserStatusCommand(UserStatusDtoRq Request) : IRequest<Unit>;
    public record UpdateUserStatusCommand(int Id, UserStatusDtoRq Request) : IRequest<Unit>;
    public record DeleteUserStatusesCommand(List<int> Ids) : IRequest<Unit>;
}