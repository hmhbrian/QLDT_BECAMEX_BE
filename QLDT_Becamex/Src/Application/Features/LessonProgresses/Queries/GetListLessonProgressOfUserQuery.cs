using MediatR;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Queries
{
    public record GetListLessonProgressOfUserQuery(string CourseId) : IRequest<List<AllLessonProgressDto>>;
}
