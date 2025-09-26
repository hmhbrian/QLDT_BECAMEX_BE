using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Messages
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; } = null!;
        public string? Data { get; set; }
        public string? SendType { get; set; }
        public string? SentBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}