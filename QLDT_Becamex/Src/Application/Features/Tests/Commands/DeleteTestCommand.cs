using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Commands
{
    public record DeleteTestCommand(int Id) : IRequest<string>;
}