using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? DepartmentId { get; set; }
        [Required]
        public string? DepartmentName { get; set; }
        public string? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public Department? Parent { get; set; }

        public string? Description { get; set; }
        public ICollection<ApplicationUser>? Users { get; set; }
    }
}
