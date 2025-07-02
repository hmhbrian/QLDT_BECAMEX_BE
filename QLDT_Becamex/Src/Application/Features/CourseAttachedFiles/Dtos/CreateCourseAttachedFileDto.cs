using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos
{
    public class CreateCourseAttachedFileDto
    {

        [Required]
        public string Title { get; set; } = null!;

        public IFormFile? File { get; set; } = null;
        public string? Link { get; set; } = null;
    }
}
