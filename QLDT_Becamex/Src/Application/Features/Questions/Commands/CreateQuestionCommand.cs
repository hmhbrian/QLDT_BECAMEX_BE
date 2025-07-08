using MediatR;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Questions.Commands
{
    public record CreateQuestionCommand(int TestId, List<CreateQuestionDto> Request) : IRequest<string>;
}
