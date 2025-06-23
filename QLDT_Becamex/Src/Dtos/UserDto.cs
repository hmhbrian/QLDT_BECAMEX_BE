using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Dtos
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? IdCard { get; set; }
        public string? Code { get; set; } // mã nhân viên
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Role { get; set; }
        public UserDto? ManagerU { get; set; }
        public PositionDto? Position { get; set; }
        public DepartmentDto? Department { get; set; } // Navigation property
        public UserStatusDto? UserStatus { get; set; } // Navigation property
        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifedAt { get; set; }

        public string? AccessToken { get; set; } = null;
    }

    public class UserDtoRq
    {
        [Required(ErrorMessage = "FullName is require")]
        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "IdCard must be between 10 and 50 characters.")]
        public string? IdCard { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 50 characters.")]
        public string? Code { get; set; } = null;


        public int? PositionId { get; set; }

        public string? RoleId { get; set; }

        public string? ManagerUId { get; set; }


        public int? DepartmentId { get; set; }


        public int? StatusId { get; set; }

        [StringLength(50, ErrorMessage = "Number phone cannot exceed 50 characters.")]
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

    public class UserLoginRq
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

        // Tùy chọn: dùng cho chức năng "Remember me" nếu bạn muốn duy trì phiên đăng nhập lâu hơn
    }

    public class AdminUpdateUserDtoRq
    {

        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string? FullName { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "IdCard must be between 10 and 50 characters.")]
        public string? IdCard { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 50 characters.")]
        public string? Code { get; set; } = null;


        public int? PositionId { get; set; }

        public string? RoleId { get; set; }

        public string? ManagerUId { get; set; }


        public int? DepartmentId { get; set; }


        public int? StatusId { get; set; }

        [StringLength(50, ErrorMessage = "Number phone cannot exceed 50 characters.")]
        public string? NumberPhone { get; set; } = null;
        public DateTime? StartWork { get; set; } = DateTime.Now;
        public DateTime? EndWork { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email format")]
        // Thêm RegularExpression để kiểm tra domain @becamex.com
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@becamex\.com$",
             ErrorMessage = "Email must be from @becamex.com domain.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; } = null;


        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        public string? NewPassword { get; set; } = null;

        // Thêm trường ConfirmPassword

        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; } = null;
    }

    public class UserChangePasswordRq
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

    public class UserResetPasswordRq
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; } = string.Empty;

        // Thêm trường ConfirmPassword
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class UserUpdateSelfDtoRq
    {
        [StringLength(100, ErrorMessage = "The Phone number must be at least 3 and at max 100 characters long.", MinimumLength = 3)] // Cập nhật/Thêm dòng này
        public string? FullName { get; set; }
        public IFormFile? UrlAvatar { get; set; }

        [StringLength(100, ErrorMessage = "The Phone number must be at least 10 and at max 100 characters long.", MinimumLength = 10)] // Cập nhật/Thêm dòng này
        public string? PhoneNumber { get; set; }
    }


    public class UserStatusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class UserStatusDtoRq
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
