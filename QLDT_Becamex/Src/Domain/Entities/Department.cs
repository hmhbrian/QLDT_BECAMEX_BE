

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Department
    {

        public int DepartmentId { get; set; } // Khóa chính
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public int Level { get; set; }
        public int? ParentId { get; set; } // Khóa ngoại tự tham chiếu
        public Department? Parent { get; set; } // Navigation property tới Department cha
        public string? ManagerId { get; set; }
        public ApplicationUser? Manager { get; set; }
        public string? Description { get; set; }
        public int? StatusId { get; set; }
        public DepartmentStatus? Status { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; } // Collection của các User thuộc phòng ban này
        public ICollection<Department>? Children { get; set; } // THÊM MỚI: Để quản lý các phòng ban con

        public ICollection<CourseDepartment> CourseDepartments { get; set; } = new List<CourseDepartment>();


    }
}
