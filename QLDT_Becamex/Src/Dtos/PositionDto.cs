using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos
{
    public class PositionDto
    {
        public int PositionId { get; set; } // Khóa chính
        public string? PositionName { get; set; }
    }
    public class PositionRq
    {
        [Required]
        public string PositionName { get; set; } = null!;
    }
}
