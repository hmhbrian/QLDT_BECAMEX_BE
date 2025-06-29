namespace QLDT_Becamex.Src.Application.Features.Lecturer.Dtos
{
    public class LecturerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
