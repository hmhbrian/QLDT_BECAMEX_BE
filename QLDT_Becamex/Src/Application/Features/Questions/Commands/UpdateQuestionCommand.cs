using MediatR;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Questions.Commands
{
    public record UpdateQuestionCommand(int QuestionId, int TestId, UpdateQuestionDto Request) : IRequest<string>;
}
