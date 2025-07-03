using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Commands
{
    public record CreateLessonCommand(string CourseId, CreateLessonDto Request) : IRequest;
}
