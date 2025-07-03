using MediatR;
using static QLDT_Becamex.Src.Application.Features.Tests.Dtos.TestReponseDTO;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetListTestOfCourseQuery(string CourseId) : IRequest<List<AllTestDto>>;
}
