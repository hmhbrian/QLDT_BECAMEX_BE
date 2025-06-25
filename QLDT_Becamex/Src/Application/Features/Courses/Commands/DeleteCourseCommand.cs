using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Courses.Commands
{
    public record DeleteCourseCommand(string Id) : IRequest<string>;
}
