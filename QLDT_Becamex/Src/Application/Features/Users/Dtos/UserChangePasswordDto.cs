using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserChangePasswordDto
    {

        [Required(ErrorMessage = "Old password is required.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required.")] // Changed message for clarity
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The new password must be at least 6 and at max 100 characters long.", MinimumLength = 6)] // Clarified message
        public string NewPassword { get; set; } = string.Empty;

        // Corrected: Compare with "NewPassword"
        [Required(ErrorMessage = "Confirm new password is required.")] // Changed message for clarity
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
