using QLDT_Becamex.Src.Constant;
using System.ComponentModel.DataAnnotations;

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

        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationStartDate { get; set; } = DateTime.Now;
        public DateTime? RegistrationClosingDate { get; set; } = DateTime.Now;
        public string? Location { get; set; }
        public int? StatusId { get; set; }
        public List<int>? DepartmentIds { get; set; }
        public List<int>? PositionIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RegistrationStartDate == null)
            {
                yield return new ValidationResult(
                    "Vui lòng nhập ngày bắt đầu đăng ký.",
                    new[] { nameof(RegistrationStartDate) });
            }
            if (RegistrationClosingDate == null)
            {
                yield return new ValidationResult(
                    "Vui lòng nhập hạn đăng ký.",
                    new[] { nameof(RegistrationStartDate) });
            }
            if (StartDate == null)
            {
                yield return new ValidationResult(
                    "Vui lòng nhập ngày bắt đầu học.",
                    new[] { nameof(RegistrationStartDate) });
            }
            if (EndDate == null)
            {
                yield return new ValidationResult(
                    "Vui lòng nhập ngày hoàn thành khóa học.",
                    new[] { nameof(RegistrationStartDate) });
            }
            if (RegistrationStartDate.HasValue && RegistrationClosingDate.HasValue && StartDate.HasValue && EndDate.HasValue)
            {
                
                if (RegistrationStartDate.Value >= StartDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày bắt đầu đăng ký phải trước ngày bắt đầu khóa học",
                        new[] { nameof(RegistrationStartDate) });
                }

                if (RegistrationClosingDate.Value > StartDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày kết thúc đăng ký phải trước hoặc bằng ngày bắt đầu khóa học",
                        new[] { nameof(RegistrationClosingDate) });
                }

                if (RegistrationStartDate.Value >= RegistrationClosingDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày bắt đầu đăng ký phải trước ngày kết thúc đăng ký",
                        new[] { nameof(RegistrationStartDate), nameof(RegistrationClosingDate) });
                }

                if (StartDate.Value >= EndDate.Value)
                {
                    yield return new ValidationResult(
                        "Ngày bắt đầu khóa học phải trước ngày kết thúc",
                        new[] { nameof(StartDate), nameof(EndDate) });
                }
            }    
        }
    }



}
