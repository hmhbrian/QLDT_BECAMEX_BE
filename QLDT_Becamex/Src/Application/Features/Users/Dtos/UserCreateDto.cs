using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "FullName is require")]
        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "IdCard must be between 10 and 50 characters.")]
        public string? IdCard { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 50 characters.")]
        public string? Code { get; set; } = null;
        public string? Position { get; set; } = null;
        public int? ELevelId { get; set; }

        public string? RoleId { get; set; }

        public string? ManagerUId { get; set; }


        public int? DepartmentId { get; set; }


        public int? StatusId { get; set; }

        [Required(ErrorMessage = "PhoneNumber is require")]
        [StringLength(10, ErrorMessage = "Number phone cannot exceed 10 characters.")]
        public string? NumberPhone { get; set; } = null;
        public DateTime? StartWork { get; set; } = DateTime.Now;
        public DateTime? EndWork { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        // Thêm RegularExpression để kiểm tra domain @becamex.com
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@becamex\.com$",
             ErrorMessage = "Email must be from @becamex.com domain.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        // Thêm trường ConfirmPassword
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
