using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetAllTestQuery : IRequest<List<TestDto>>;
}