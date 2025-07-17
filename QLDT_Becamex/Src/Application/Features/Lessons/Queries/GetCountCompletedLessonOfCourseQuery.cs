using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Queries
{
    public record GetCountCompletedLessonOfCourseQuery(string CourseId) : IRequest<int>;
}
