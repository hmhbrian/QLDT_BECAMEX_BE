namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public string? CourseId { get; set; }
        public Course? Course { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int q1_revelance { get; set; }
        public int q2_clarity { get; set; }
        public int q3_structure { get; set; }
        public int q4_duration { get; set; }
        public int q5_material { get; set; }
        public string? Comment { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}