using MediatR;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;

namespace QLDT_Becamex.Src.Application.Features.CourseCategory.Queries
{
    public record GetAlCourseCategoryQuery : IRequest<IEnumerable<CourseCategoryDto>>;
}
