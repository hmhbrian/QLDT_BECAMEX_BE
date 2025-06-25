using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Positions.Dtos
{
    public class PositionDto
    {
        public int PositionId { get; set; } // Khóa chính
        public string? PositionName { get; set; }
    }
    public class CreatePositionDto
    {
        [Required]
        public string PositionName { get; set; } = null!;
    }
}
