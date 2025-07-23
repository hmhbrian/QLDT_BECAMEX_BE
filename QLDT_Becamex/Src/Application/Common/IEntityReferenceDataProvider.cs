using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Common
{
    public interface IEntityReferenceDataProvider
    {
        Task<ReferenceData> GetReferenceData(AuditLog auditLog);
    }
}
