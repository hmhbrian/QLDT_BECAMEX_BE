namespace QLDT_Becamex.Src.Dtos.Users
{
    public class UserUpdateSelfDto
    {
        public string? FullName { get; set; }
        public IFormFile? UrlAvatar { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
