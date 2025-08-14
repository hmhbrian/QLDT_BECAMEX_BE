using MediatR;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetCountCompletedTestOfCourseQuery(string CourseId) : IRequest<int>;
}
