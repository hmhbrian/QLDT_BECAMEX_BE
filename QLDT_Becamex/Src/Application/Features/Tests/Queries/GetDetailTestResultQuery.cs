using MediatR;
using QLDT_Becamex.Src.Application.Features.Tests.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Queries
{
    public record GetDetailTestResultQuery(int Id, string CourseId) : IRequest<DetailTestResultDto>;
}