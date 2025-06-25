using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Common.Dtos
{
    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? Description { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public string? Status { get; set; }
        public int Level { get; set; }
        public List<string>? Path { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DepartmentDto>? Children { get; set; }
    }

    public class DepartmentRq
    {
        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department code is required")]
        [MaxLength(50, ErrorMessage = "Department code cannot exceed 50 characters")]
        public string DepartmentCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        //[Required(ErrorMessage = "Level is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "Level must be at least 1")]
        //public int Level { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [MaxLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; } = "active";
        public string ManagerId { get; set; } = string.Empty;
        public int? ParentId { get; set; }
    }
}
