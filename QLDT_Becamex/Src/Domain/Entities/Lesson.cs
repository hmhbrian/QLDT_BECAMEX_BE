using System.Runtime.CompilerServices;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public string Title { get; set; }
        public string UrlPdf { get; set; }
        public int? Order { get; set; }
        public string? UserIdCreated { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
