using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseELevel
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public int ELevelId { get; set; }
        [JsonIgnore]
        public Course Course { get; set; } = null!;
        public EmployeeLevel ELevel { get; set; } = null!;
    }
}
