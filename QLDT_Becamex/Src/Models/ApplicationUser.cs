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

        public DateTime? StartWork { get; set; }

        public DateTime? EndWork { get; set; }

        public string? Status { get; set; }

        public string? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        public string? PositionId { get; set; }
        [ForeignKey("PositionId")]
        public Position? Position { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifedAt { get; set; }

    }

}