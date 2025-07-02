using MediatR;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Queries
{
    public record GetListLessonOfCourseQuery(string CourseId) : IRequest<List<AllLessonDto>>;
}
