// Ví dụ: Src/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace QLDT_Becamex.Src.Domain.Entities // Đảm bảo namespace này khớp với nơi bạn định nghĩa ApplicationUser
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? NormalizedFullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? IdCard { get; set; }
        public string? Code { get; set; } // mã nhân viên
        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public int? StatusId { get; set; }
        public UserStatus? UserStatus { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Position { get; set; }
        public string? ManagerUId { get; set; } // Khóa ngoại tới ApplicationUser khác (không cần [ForeignKey] nữa)
        public ApplicationUser? ManagerU { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; } // Navigation property
        public int? ELevelId { get; set; }
        public EmployeeLevel? ELevel { get; set; } // Navigation property
        public string? CreateById { get; set; }
        public ApplicationUser? CreateBy { get; set; }
        public string? UpdateById { get; set; }
        public ApplicationUser? UpdateBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public ICollection<ApplicationUser> Children { get; set; } = new List<ApplicationUser>();
        public ICollection<UserCourse> UserCourse { get; set; } = new List<UserCourse>();
        public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
        public ICollection<Certificates> Certificates { get; set; } = new List<Certificates>();

    }

}