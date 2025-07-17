using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetTestResultQuery(int Id, string CourseId) : IRequest<TestResultDto>;
}