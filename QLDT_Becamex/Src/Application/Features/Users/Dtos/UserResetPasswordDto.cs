using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserResetPasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        // Thêm trường ConfirmPassword
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
