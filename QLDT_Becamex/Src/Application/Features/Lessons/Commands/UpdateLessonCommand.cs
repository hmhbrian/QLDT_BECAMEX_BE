using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Commands
{
    public record UpdateLessonCommand(string CourseId, int LessonId, UpdateLessonDto Request) : IRequest;
}