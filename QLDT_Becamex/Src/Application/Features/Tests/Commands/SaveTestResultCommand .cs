using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Commands
{
    public record SaveTestResultCommand(int TestId, string CourseId, DateTime StartedAt, List<UserAnswerDto> SubmittedAnswers) : IRequest<TestResultDto>;
}
