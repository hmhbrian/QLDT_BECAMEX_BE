using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace QLDT_Becamex.Src.Models
{
    public class CourseSatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Course>? Courses { get; set; } = new List<Course>();


    }
}
