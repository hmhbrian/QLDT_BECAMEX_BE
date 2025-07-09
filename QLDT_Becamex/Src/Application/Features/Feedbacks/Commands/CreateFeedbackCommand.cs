using MediatR;
using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Commands
{
    public record CreateFeedbackCommand(CreateFeedbackDto Request, string CourseId) : IRequest<string>;
}