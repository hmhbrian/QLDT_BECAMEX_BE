namespace QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos
{
    public class UpsertLessonProgressDto
    {
        public int LessonId { get; set; }
        public int? CurrentTimeSecond { get; set; } // Thời gian hiện tại của video (tính bằng giây)
        public int? CurrentPage { get; set; } // Trang hiện tại của tài liệu PDF
    }
}
