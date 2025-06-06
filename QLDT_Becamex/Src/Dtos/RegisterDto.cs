// Src/Dtos/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "FullName is require")]
        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "IdCard is require")]
        [MinLength(10, ErrorMessage = "IdCard must be at least 10 characters.")]
        [StringLength(50, ErrorMessage = "IdCard cannot exceed 50 characters.")]
        public string IdCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is require")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 50 characters.")]
        public string Role { get; set; } = string.Empty;

        [Required(ErrorMessage = "NumberPhone is require")]
        [MinLength(10, ErrorMessage = "Phone number must be at least 10 characters.")]
        [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters.")]
        public string NumberPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        // Thêm RegularExpression để kiểm tra domain @becamex.com
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@becamex\.com$",
             ErrorMessage = "Email must be from @becamex.com domain.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]

        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        // Thêm trường ConfirmPassword
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Tùy chọn: dùng cho chức năng "Remember me" nếu bạn muốn duy trì phiên đăng nhập lâu hơn
    }
}