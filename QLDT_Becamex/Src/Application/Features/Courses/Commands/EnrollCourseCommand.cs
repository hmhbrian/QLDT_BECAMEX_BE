using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Courses.Commands
{
    public record EnrollCourseCommand(string CourseId) : IRequest<string>;
}
