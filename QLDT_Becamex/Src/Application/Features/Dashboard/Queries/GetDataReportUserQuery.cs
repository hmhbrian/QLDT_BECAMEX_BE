using MediatR;
using QLDT_Becamex.Src.Application.Features.Dashboard.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Dashboard.Queries
{
    public record GetDashBoardUserQuery() : IRequest<DataReportUserDto>;
}
