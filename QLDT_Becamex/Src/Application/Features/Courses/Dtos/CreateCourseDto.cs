using QLDT_Becamex.Src.Constant;

namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    using System.ComponentModel.DataAnnotations;

    public class CreateCourseDto : IValidatableObject
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

        [Required]
        [Range(1, 200, ErrorMessage = "Số lượng học viên phải lớn hơn không và nhỏ hon 200")]
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsPrivate { get; set; } = false;
        public List<int>? DepartmentIds { get; set; }
        public List<int>? ELevelIds { get; set; }
        public List<string>? UserIds { get; set; }



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
