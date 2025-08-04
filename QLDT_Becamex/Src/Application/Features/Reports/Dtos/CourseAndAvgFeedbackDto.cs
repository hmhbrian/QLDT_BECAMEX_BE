using QLDT_Becamex.Src.Application.Features.Feedbacks.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Reports.Dtos
{
    public class AvgFeedbackDto
    {
        public float Q1_relevanceAvg { get; set; }
        public float Q2_clarityAvg { get; set; }
        public float Q3_structureAvg { get; set; }
        public float Q4_durationAvg { get; set; }
        public float Q5_materialAvg { get; set; }
    }
    public class CourseAndAvgFeedbackDto
    {
        public string courseName { get; set; } = null!;
        public AvgFeedbackDto AvgFeedback { get; set; } = new AvgFeedbackDto();
        public float avgFeedbackScore => (AvgFeedback.Q1_relevanceAvg + AvgFeedback.Q2_clarityAvg + AvgFeedback.Q3_structureAvg + AvgFeedback.Q4_durationAvg + AvgFeedback.Q5_materialAvg) / 5;
    }
}