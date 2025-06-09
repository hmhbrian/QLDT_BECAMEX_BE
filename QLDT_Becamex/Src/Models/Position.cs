using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models
{
    public class Position
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? PositionId { get; set; }
        [Required]
        public string? PositionName { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
