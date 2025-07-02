

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseAttachedFile
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Type { get; set; } = null!;
        public string? Link { get; set; }
        public string? PublicIdUrlPdf { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser UserCreated { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedTime { get; set; } = DateTime.Now;

    }
}
