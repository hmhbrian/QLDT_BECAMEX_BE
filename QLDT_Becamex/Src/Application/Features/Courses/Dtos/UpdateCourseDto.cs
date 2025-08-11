using QLDT_Becamex.Src.Constant;
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UpdateCourseDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public IFormFile? ThumbUrl { get; set; }

        [RegularExpression("^(online|offline)$", ErrorMessage = "Giá trị chỉ được là 'online' hoặc 'offline'.")]
        public string? Format { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }

        [RegularExpression($"^({ConstantCourse.OPTIONAL_TUYCHON}|{ConstantCourse.OPTIONAL_BATBUOC})$",
            ErrorMessage = "Giá trị chỉ được là 'tùy chọn' hoặc 'bắt buộc'.")]
        public string? Optional { get; set; }
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsPrivate { get; set; }
        public List<int> DepartmentIds { get; set; } = new List<int>();
        public List<int> ELevelIds { get; set; } = new List<int>();
        public List<string> UserIds { get; set; } = new List<string>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegistrationStartDate.HasValue && StartDate.HasValue &&
                RegistrationStartDate.Value >= StartDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày bắt đầu đăng ký phải trước ngày bắt đầu khóa học",
                    new[] { nameof(RegistrationStartDate) });
            }

            if (RegistrationClosingDate.HasValue && StartDate.HasValue &&
                RegistrationClosingDate.Value > StartDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc đăng ký phải trước hoặc bằng ngày bắt đầu khóa học",
                    new[] { nameof(RegistrationClosingDate) });
            }

            if (RegistrationStartDate.HasValue && RegistrationClosingDate.HasValue &&
                RegistrationStartDate.Value >= RegistrationClosingDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày bắt đầu đăng ký phải trước ngày kết thúc đăng ký",
                    new[] { nameof(RegistrationStartDate), nameof(RegistrationClosingDate) });
            }

            if (StartDate.HasValue && EndDate.HasValue &&
                StartDate.Value >= EndDate.Value)
            {
                yield return new ValidationResult(
                    "Ngày bắt đầu khóa học phải trước ngày kết thúc",
                    new[] { nameof(StartDate), nameof(EndDate) });
            }
        }
    }
}
