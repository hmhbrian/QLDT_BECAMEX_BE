using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Commands
{
    public record UpdateTestCommand(int Id, TestUpdateDto Request, string CourseId) : IRequest<string>;
}