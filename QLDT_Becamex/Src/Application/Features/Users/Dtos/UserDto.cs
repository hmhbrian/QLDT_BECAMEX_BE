
using QLDT_Becamex.Src.Application.Features.Departments.Dtos;
using QLDT_Becamex.Src.Application.Features.EmployeeLevels.Dtos;
using QLDT_Becamex.Src.Application.Features.Status.Dtos;
using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
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
        public string? Position { get; set; }
        public UserSumaryDto? CreatedBy { get; set; }
        public UserSumaryDto? UpdatedBy { get; set; }
        public UserSumaryDto? ManagerBy { get; set; }
        public ELevelDto? ELevel { get; set; }
        public DepartmentShortenDto? Department { get; set; } // Navigation property
        public StatusDto? UserStatus { get; set; }
        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? AccessToken { get; set; } = null;
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
