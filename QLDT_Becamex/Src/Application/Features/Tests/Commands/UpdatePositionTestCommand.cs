using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Tests.Commands
{
    public record UpdatePositionTestCommand(string CourseId, int TestId, int PreviousTestId) : IRequest;
}
