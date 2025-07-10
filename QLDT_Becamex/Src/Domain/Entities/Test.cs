namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Test
    {
        public int Id { get; set; }
        public string? CourseId { get; set; }
        public Course? Course { get; set; }
        public string? Title { get; set; }
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
        public int Position { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
    }
}
