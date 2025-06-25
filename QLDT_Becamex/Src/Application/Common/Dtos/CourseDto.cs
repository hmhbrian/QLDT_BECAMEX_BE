using QLDT_Becamex.Src.Constant;
using QLDT_Becamex.Src.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Dtos
{
    public class CourseDto
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }
        public string? Optional { get; set; }
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public int? StatusId { get; set; }
        public CourseStatus? Status { get; set; }
        public ICollection<DepartmentDto>? Departments { get; set; } = new List<DepartmentDto>();
        public ICollection<PositionDto>? Positions { get; set; } = new List<PositionDto>();
    }

    public class CourseDtoRq
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        [Required]
        public string Objectives { get; set; } = null!;
        public IFormFile? ThumbUrl { get; set; }

        [RegularExpression("^(online|offline)$", ErrorMessage = "Giá trị chỉ được là 'online' hoặc 'offline'.")]
        public string? Format { get; set; } = "online";
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }

        [RegularExpression($"^({ConstantCourse.OPTIONAL_TUYCHON}|{ConstantCourse.OPTIONAL_BATBUOC})$",
                             ErrorMessage = "Giá trị chỉ được là 'tùy chọn' hoặc 'bắt buộc'.")]
        public string? Optional { get; set; } = ConstantCourse.OPTIONAL_TUYCHON;


        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationStartDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationClosingDate { get; set; } = DateTime.Now;
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<int>? PositionIds { get; set; }
    }


    public class CourseStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CourseStatusDtoRq
    {
        [Required]
        public string Name { get; set; } = null!;
    }

}