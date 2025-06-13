

using Microsoft.AspNetCore.Identity;

namespace QLDT_Becamex.Src.Models
{
    public class Position
    {
        public int? PositionId { get; set; } // Khóa chính
        public string? PositionName { get; set; }



        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User có vị trí này
    }
}
