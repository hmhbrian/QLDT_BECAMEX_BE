using QLDT_Becamex.Src.Constant;
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos.Courses
{
    public class CourseDtoRq
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        [Required]
        public string Objecttives { get; set; } = null!;
        public IFormFile? ThumbUrl { get; set; }

        [RegularExpression("^(online|offline)$", ErrorMessage = "Giá trị chỉ được là 'online' hoặc 'offline'.")]
        public string? Format { get; set; } = "online";
        public int? Sesstions { get; set; }
        public int? HoursPerSesstions { get; set; }

        [RegularExpression($"^({ConstantCourse.OPTIONAL_TUYCHON}|{ConstantCourse.OPTIONAL_BATBUOC})$",
                             ErrorMessage = "Giá trị chỉ được là 'tùy chọn' hoặc 'bắt buộc'.")]
        public string? Optional { get; set; } = ConstantCourse.OPTIONAL_TUYCHON;


        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationStartDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationSlosingDate { get; set; } = DateTime.Now;
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<int>? PositionIds { get; set; }
    }

}
