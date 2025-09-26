using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Topics
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}