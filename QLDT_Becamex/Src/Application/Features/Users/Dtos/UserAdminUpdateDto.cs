using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserAdminUpdateDto
    {


        [StringLength(50, ErrorMessage = "FullName cannot exceed 50 characters.")]
        public string? FullName { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "IdCard must be between 10 and 50 characters.")]
        public string? IdCard { get; set; } = null;

        [StringLength(50, MinimumLength = 10, ErrorMessage = "Code must be between 10 and 50 characters.")]
        public string? Code { get; set; } = null;

        public string? Position { get; set; }
        public int? ELevelId { get; set; }

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



    }
}
