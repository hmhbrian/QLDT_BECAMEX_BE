using MediatR;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;
namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Queries
{
    public record GetListFeedbackQuery(string CourseId) : IRequest<List<FeedbacksDto>>;
}