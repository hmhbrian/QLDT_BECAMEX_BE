using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CreateDeviceDto
    {
        public string? UserId { get; set; }
        public string? DeviceToken { get; set; }
        public string? Platform { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}