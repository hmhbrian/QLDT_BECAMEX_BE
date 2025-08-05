using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetTestNoAnswerQuery(string courseId, int TestId) : IRequest<List<QuestionNoAnswerDto>>;

}