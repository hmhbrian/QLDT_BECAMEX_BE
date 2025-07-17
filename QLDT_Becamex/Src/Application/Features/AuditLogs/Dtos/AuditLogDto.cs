using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.AuditLogs.Dtos
{
    public class AuditLogDto
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? UserName { get; set; }
        public string? Timestamp { get; set; }
        public List<ChangedField>? ChangedFields { get; set; }
        public List<AddedField>? AddedFields { get; set; }
        public List<DeletedField>? DeletedFields { get; set; }
    }

    public class ChangedField
    {
        public string? FieldName { get; set; }
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }
    public class AddedField
    {
        public string? FieldName { get; set; }
        public object? Value { get; set; }
    }
    public class DeletedField
    {
        public string? FieldName { get; set; }
        public object? Value { get; set; }
    }
}
