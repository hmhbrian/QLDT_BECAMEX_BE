using MediatR;
using static QLDT_Becamex.Src.Application.Features.Lessons.Dtos.LessonResponseDTO;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Queries
{
    public record GetListLessonOfCourseQuery(string CourseId) : IRequest<List<AllLessonDto>>;
}
