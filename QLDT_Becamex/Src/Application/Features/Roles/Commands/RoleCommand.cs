using MediatR;
using QLDT_Becamex.Src.Application.Features.Roles.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Roles.Commands
{
    public record CreateRoleCommand(CreateRoleDto Request) : IRequest<string>;
    public record UpdateRoleCommand(string Id, CreateRoleDto Request) : IRequest<string>;
    public record DeleteRoleCommand(string Id) : IRequest<string>;
}