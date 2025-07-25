using MediatR;
using QLDT_Becamex.Src.Application.Common.Dtos;
using QLDT_Becamex.Src.Application.Features.Courses.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Queries
{
    public record GetListUserCoursesProgressQuery(BaseQueryParam baseQueryParam, string courseId) : IRequest<PagedResult<UserCourseProgressDto>>;
}