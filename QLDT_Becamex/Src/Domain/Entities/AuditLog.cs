namespace QLDT_Becamex.Src.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? EntityName { get; set; }
        public string? EntityId { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Changes { get; set; } // JSON chứa old/new values của entity
    }
}
