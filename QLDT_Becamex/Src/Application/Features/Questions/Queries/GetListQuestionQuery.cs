using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Questions.Queries
{
    public record GetListQuestionQuery(int TestId, BaseQueryParam BaseQueryParam) : IRequest<PagedResult<QuestionDto>>;

}
