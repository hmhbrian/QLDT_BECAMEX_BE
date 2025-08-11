using MediatR;
using QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos;

namespace QLDT_Becamex.Src.Application.Features.CourseCategory.Commands
{
    public record CreateCourseCategoryCommand(CourseCategoryRqDto Request) : IRequest<Unit>;
    public record UpdateCourseCategoryCommand(int id, CourseCategoryRqDto Request) : IRequest<Unit>;
    public record DeleteCourseCategoryCommand(List<int> Ids) : IRequest<Unit>;
}
