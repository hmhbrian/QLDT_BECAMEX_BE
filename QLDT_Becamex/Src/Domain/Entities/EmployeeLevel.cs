
using Microsoft.AspNetCore.Identity;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class EmployeeLevel
    {
        public int ELevelId { get; set; } // Khóa chính
        public string? ELevelName { get; set; }

        public ICollection<CourseELevel> CourseELevel { get; set; } = new List<CourseELevel>();

        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User có vị trí này
    }
}
