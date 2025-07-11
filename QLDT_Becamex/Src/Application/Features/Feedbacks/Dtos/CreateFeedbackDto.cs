namespace QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos
{
    public class CreateFeedbackDto
    {
        public int Q1_relevance { get; set; }
        public int Q2_clarity { get; set; }
        public int Q3_structure { get; set; }
        public int Q4_duration { get; set; }
        public int Q5_material { get; set; }
        public string? Comment { get; set; }
    }
}