using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseCategory
    {
        public int Id { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public ICollection<Course>? Courses { get; set; } = new List<Course>();
    }
}
