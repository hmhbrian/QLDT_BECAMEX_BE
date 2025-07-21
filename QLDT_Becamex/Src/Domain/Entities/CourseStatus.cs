using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json.Serialization;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseStatus
    {
        public int Id { get; set; }
        public int Key { get; set; }
        public string StatusName { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Course>? Courses { get; set; } = new List<Course>();
    }
}
