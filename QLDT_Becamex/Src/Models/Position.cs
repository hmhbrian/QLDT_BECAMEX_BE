using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models
{
    public class Position
    {
        public string? PositionId { get; set; } // Khóa chính
        public string? PositionName { get; set; }
        public string? Description { get; set; } // Thêm Description nếu bạn muốn có nó trong model
        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User có vị trí này
    }
}
