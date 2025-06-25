using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Users.Dtos
{
    public class UserUserUpdateDto
    {
        [StringLength(100, ErrorMessage = "The Phone number must be at least 3 and at max 100 characters long.", MinimumLength = 3)] // Cập nhật/Thêm dòng này
        public string? FullName { get; set; }
        public IFormFile? UrlAvatar { get; set; }

        [StringLength(100, ErrorMessage = "The Phone number must be at least 10 and at max 100 characters long.", MinimumLength = 10)] // Cập nhật/Thêm dòng này
        public string? PhoneNumber { get; set; }
    }
}
