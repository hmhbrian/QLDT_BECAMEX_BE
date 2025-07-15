using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Commands
{
    public record UpdatePositionLessonCommand(string CourseId, int LessonId, int? PreviousLessonId) : IRequest;
}
