using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Questions.Commands
{
    public record DeleteQuestionCommand(int TestId, int QuestionId) : IRequest<string>;
}
