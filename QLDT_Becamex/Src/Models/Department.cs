using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models
{
    public class Department
    {

        public string? DepartmentId { get; set; } // Khóa chính
        public string? DepartmentName { get; set; }
        public string? ParentId { get; set; } // Khóa ngoại tự tham chiếu
        public Department? Parent { get; set; } // Navigation property tới Department cha
        public string? Description { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User thuộc phòng ban này
        public ICollection<Department>? Children { get; set; } // THÊM MỚI: Để quản lý các phòng ban con
    }
}
