using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetListTestOfCourseQuery(string CourseId) : IRequest<List<AllTestDto>>;
}
