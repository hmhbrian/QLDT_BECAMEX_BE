using QLDT_Becamex.Src.Dtos.Positions;
using QLDT_Becamex.Src.Models;

namespace QLDT_Becamex.Src.Dtos.Users
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? IdCard { get; set; }
        public string? Code { get; set; } // mã nhân viên
        public string? Status { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Role { get; set; }
        public PositionDto? Position { get; set; }
        public DateTime? StartWork { get; set; }
        public DateTime? EndWork { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifedAt { get; set; }
    }
}
