using System.Runtime.CompilerServices;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public string? course_id { get; set; }
        public Course? Course { get; set; }
        public string? title { get; set; }
        public string? content_pdf { get; set; }
        public int Order { get; set; }
        public string? userId_created { get; set; } 
        public ApplicationUser? UserCreated { get; set; }
        public string? userId_edited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
