using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Courses.Commands
{
    public record CancelEnrollCourseCommand(string CourseId) : IRequest<string>;
}