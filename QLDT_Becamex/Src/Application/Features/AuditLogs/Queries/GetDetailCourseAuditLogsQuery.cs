using MediatR;
using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.Queries
{
    public record GetDetailCourseAuditLogsQuery(string courseId) : IRequest<List<AuditLogDto>>;
}
