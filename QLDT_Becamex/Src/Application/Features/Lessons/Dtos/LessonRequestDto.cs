using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Dtos
{
    public class CreateLessonDto
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
        public string Title { get; set; } = null!;
        public IFormFile? FilePdf { get; set; } = null;
        public string? Link { get; set; } = null;
        public int TotalDurationSeconds { get; set; } = 0; // Tổng thời gian của video (nếu có)
    }

    public class UpdateLessonDto
    {
        public string? Title { get; set; }

        public IFormFile? FilePdf { get; set; }
        public string? Link { get; set; } = null;
        public int TotalDurationSeconds { get; set; } = 0;
    }
}
