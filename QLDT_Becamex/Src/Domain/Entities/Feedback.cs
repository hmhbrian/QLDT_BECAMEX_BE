namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Feedback
    {
        public int Id { get; set; }
        public string? CourseId { get; set; }
        public Course? Course { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public int Q1_relevance { get; set; }
        public int Q2_clarity { get; set; }
        public int Q3_structure { get; set; }
        public int Q4_duration { get; set; }
        public int Q5_material { get; set; }
        public string? Comment { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}