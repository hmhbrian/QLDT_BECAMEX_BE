using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Common.Mappings.AuditLogs
{
    public interface IAuditLogMapper
    {
        Task<AuditLogDto> MapToDto(AuditLog auditLog, Dictionary<string, ApplicationUser> userDict, Dictionary<string, IEntityReferenceDataProvider> referenceDataProviders);
    }
}
