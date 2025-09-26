using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class TopicSubscriptions
    {
        public int Id { get; set; }
        public int? TopicId { get; set; }
        public int? DeviceId { get; set; } = null!;
        public DateTime SubscribeAt { get; set; }
        public Topics? topics { get; set; }
        public Device? devices { get; set; }
    }
}