// Ví dụ: Src/Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLDT_Becamex.Src.Models // Đảm bảo namespace này khớp với nơi bạn định nghĩa ApplicationUser
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? IdCard { get; set; }
        public string? Code { get; set; } // mã nhân viên
        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public string? Status { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string? ManagerUId { get; set; } // Khóa ngoại tới ApplicationUser khác (không cần [ForeignKey] nữa)
        public ApplicationUser? managerU { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; } // Navigation property

        public int? PositionId { get; set; }
        public Position? Position { get; set; } // Navigation property


        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifedAt { get; set; }
        public ICollection<ApplicationUser>? Children { get; set; }

    }

}