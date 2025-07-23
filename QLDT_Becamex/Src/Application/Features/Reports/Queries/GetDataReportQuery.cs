using MediatR;
using QLDT_Becamex.Src.Application.Features.Reports.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Reports.Queries
{
    public record GetDataReportQuery(int? month, int? quarter, int? year) : IRequest<DataReportDto>;
}