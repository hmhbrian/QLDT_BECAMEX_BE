using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace QLDT_Becamex.Src.Shared.Helpers
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string? UserId { get; set; }
        public string? TableName { get; set; }
        public string? Action { get; set; }
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public List<PropertyEntry> TemporaryProperties { get; } = new();

        public string ToJsonChanges()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            return JsonSerializer.Serialize(new { OldValues, NewValues }, options);
        }

        public string? GetPrimaryKeyAsString()
        {
            var pk = Entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            return pk?.CurrentValue?.ToString();
        }
    }
}
