using QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos;

namespace QLDT_Becamex.Src.Application.Common
{
    public class ReferenceData
    {
        public List<AddedField> AddedFields { get; set; } = new List<AddedField>();
        public List<ChangedField> ChangedFields { get; set; } = new List<ChangedField>();
        public List<DeletedField> DeletedFields { get; set; } = new List<DeletedField>();
    }
}
