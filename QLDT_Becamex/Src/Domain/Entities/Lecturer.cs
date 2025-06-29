using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Lecturer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        [JsonIgnore]
        // Khóa học giảng dạy
        public ICollection<Course>? Courses { get; set; } = new List<Course>();

    }
}
