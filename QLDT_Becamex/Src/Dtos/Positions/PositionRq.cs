using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.Positions
{
    public class PositionRq
    {
        [Required]
        public string PositionName { get; set; } = null!;
        [Required]
        public string RoleId { get; set; } = null!;
    }
}
