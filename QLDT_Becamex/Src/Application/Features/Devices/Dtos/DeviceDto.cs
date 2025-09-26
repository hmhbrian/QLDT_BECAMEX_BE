using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Application.Features.Devices.Dtos
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? DeviceToken { get; set; }
        public string? Platform { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}