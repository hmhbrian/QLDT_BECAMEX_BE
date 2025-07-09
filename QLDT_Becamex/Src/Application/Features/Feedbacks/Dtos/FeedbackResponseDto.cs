namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos
{
    public class FeedbackDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? CourseId { get; set; }
        public int q1_revelance { get; set; }
        public int q2_clarity { get; set; }
        public int q3_structure { get; set; }
        public int q4_duration { get; set; }
        public int q5_material { get; set; }
        public string? Comment { get; set; }
    }
    public class FeedbacksDto
    {
        public int q1_revelance { get; set; }
        public int q2_clarity { get; set; }
        public int q3_structure { get; set; }
        public int q4_duration { get; set; }
        public int q5_material { get; set; }
        public string? Comment { get; set; }
    }
}