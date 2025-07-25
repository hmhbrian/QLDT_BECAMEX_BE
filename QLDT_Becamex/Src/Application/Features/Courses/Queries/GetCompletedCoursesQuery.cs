using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Queries
{
    public record GetCompletedCoursesQuery(BaseQueryParam baseQueryParam) : IRequest<PagedResult<UserEnrollCompletedCourseDto>>;
}