namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Test
    {
        public int Id { get; set; }
        public string? Course_id { get; set; }
        public Course? Course { get; set; }
        public string? Title { get; set; }
        public float Pass_threshold { get; set; }
        public int Time_test { get; set; }
        public string? UserId_created { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? UserId_edited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Tests { get; set; } = new List<Question>();
    }
}
