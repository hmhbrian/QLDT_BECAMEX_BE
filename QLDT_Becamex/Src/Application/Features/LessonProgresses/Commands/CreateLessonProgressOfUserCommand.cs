using MediatR;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Commands
{
    public record CreateLessonProgressOfUserCommand(UpsertLessonProgressDto Request) : IRequest;

}
