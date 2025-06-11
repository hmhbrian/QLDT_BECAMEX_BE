using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models
{
    public class Department
    {

        public string? DepartmentId { get; set; } // Khóa chính
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public int level { get; set; }
        public string? ParentId { get; set; } // Khóa ngoại tự tham chiếu
        public Department? Parent { get; set; } // Navigation property tới Department cha
        public string? ManagerId { get; set; }
        public ApplicationUser? manager { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User thuộc phòng ban này
        public ICollection<Department>? Children { get; set; } // THÊM MỚI: Để quản lý các phòng ban con
    }
}
