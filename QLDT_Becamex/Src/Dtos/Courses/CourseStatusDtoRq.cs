using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.Courses
{
    public class CourseStatusDtoRq
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
