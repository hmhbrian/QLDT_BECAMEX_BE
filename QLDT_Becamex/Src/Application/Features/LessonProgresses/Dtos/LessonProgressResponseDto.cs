namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos
{
    public class AllLessonProgressDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? UrlPdf { get; set; }
        public double ProgressPercentage { get; set; } // Tỷ lệ hoàn thành bài học (0-100%)
    }
}
