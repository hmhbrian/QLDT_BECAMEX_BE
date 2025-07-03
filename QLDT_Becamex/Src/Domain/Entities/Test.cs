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
        public string? UserIdCreated { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
    }
}
