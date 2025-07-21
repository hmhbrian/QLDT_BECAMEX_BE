using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Common
{
    public interface IEntityReferenceDataProvider
    {
        ReferenceData GetReferenceData(AuditLog auditLog);
    }
}
