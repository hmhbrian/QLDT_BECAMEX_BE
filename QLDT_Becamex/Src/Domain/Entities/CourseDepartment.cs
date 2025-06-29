using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseDepartment
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public int DepartmentId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; } = null!;
        public Department Department { get; set; } = null!;

    }
}
