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
        public string UserId { get; set; }
        public string TableName { get; set; }
        public string Action { get; set; }
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public object PrimaryKey => Entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey())?.CurrentValue!;

        public string ToJsonChanges()
        {
            return JsonSerializer.Serialize(new { OldValues, NewValues });
        }
    }
}
