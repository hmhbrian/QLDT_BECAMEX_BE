using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserUserUpdateDto
    {
        public string? FullName { get; set; }
        public string? UrlAvatar { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
