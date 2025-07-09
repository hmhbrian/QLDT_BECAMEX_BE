namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos
{
    public class CreateFeedbackDto
    {
        public int q1_revelance { get; set; }
        public int q2_clarity { get; set; }
        public int q3_structure { get; set; }
        public int q4_duration { get; set; }
        public int q5_material { get; set; }
        public string? Comment { get; set; }
    }
}