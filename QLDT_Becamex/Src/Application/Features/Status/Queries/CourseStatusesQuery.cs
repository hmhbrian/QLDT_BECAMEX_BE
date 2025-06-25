using MediatR;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Status.Queries
{
    public record GetAllCourseStatusesQuery : IRequest<IEnumerable<CourseStatusDto>>;
}