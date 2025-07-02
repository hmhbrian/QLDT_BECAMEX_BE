using MediatR;


namespace QLDT_Becamex.Src.Application.Features.Lessons.Commands
{
    public record DeleteLessonCommand(string CourseId, List<int> LessonIds) : IRequest;
}
