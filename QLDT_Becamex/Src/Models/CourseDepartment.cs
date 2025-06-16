using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Models
{
    public class CourseDepartment
    {
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;
    }
}
