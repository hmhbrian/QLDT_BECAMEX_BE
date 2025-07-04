using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Dtos
{
    public class CreateLessonDto
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public IFormFile FilePdf { get; set; } = null!;
    }

    public class UpdateLessonDto
    {

        public string? Title { get; set; }

        public IFormFile? FilePdf { get; set; }

    }
}
