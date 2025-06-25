using MediatR;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Queries
{
    public record GetCourseByIdQuery(string Id) : IRequest<CourseDto>;

}
