using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        // Thêm RegularExpression để kiểm tra domain @becamex.com
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@becamex\.com$",
           ErrorMessage = "Email must be from @becamex.com domain.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)] // Cập nhật/Thêm dòng này
        public string Password { get; set; } = string.Empty;
    }
}
