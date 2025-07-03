namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Test
    {
        public int Id { get; set; }
        public string? course_id { get; set; }
        public Course? Course { get; set; }
        public string? title { get; set; }
        public float pass_threshold { get; set; }
        public int time_test { get; set; }
        public string? userId_created { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? userId_edited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Tests { get; set; } = new List<Question>();
    }
}
