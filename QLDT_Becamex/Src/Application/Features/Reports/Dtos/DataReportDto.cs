namespace QLDT_Becamex.Src.Application.Features.Reports.Dtos
{
    public class DataReportDto
    {
        public int NumberOfCourses { get; set; }
        public int NumberOfStudents { get; set; }
        public float AverangeCompletedPercentage { get; set; }
        public float AverangeTime { get; set; }
        public float AveragePositiveFeedback { get; set; }
    }
}