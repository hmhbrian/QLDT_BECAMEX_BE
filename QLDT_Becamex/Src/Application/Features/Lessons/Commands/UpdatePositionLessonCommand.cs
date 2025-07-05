using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Commands
{
    public record UpdatePositionLessonCommand(string CourseId, int LessonId, int PreviousLessonId) : IRequest;
}
