using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetTestByIdQuery(int Id, string CourseId) : IRequest<DetailTestDto>;
}