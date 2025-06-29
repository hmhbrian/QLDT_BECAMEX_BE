using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.CourseCategory.Dtos
{
    public class CourseCategoryRqDto
    {
        [Required(ErrorMessage = "CourseCategoryName is required")]
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
