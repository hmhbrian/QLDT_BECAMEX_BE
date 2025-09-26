using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class MessageLogs
    {
        public int Id { get; set; }
        public int? MessageId { get; set; }
        public int? DeviceId { get; set; } = null!;
        public int? TopicId { get; set; }
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime ReceivedAt { get; set; }
        public Messages? messages { get; set; }
        public Device? devices { get; set; }
        public Topics? topics { get; set; }
    }
}