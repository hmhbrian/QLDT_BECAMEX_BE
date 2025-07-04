using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Questions.Commands
{
    public record DeleteQuestionsCommand(int TestId, List<int> QuestionIds) : IRequest<string>;
}
