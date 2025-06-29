using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Lecturer.Dtos
{
    public class LecturerDtoRq
    {
        [Required]
        public string FullName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email must be a valid email address")]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
