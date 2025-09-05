using QLDT_Becamex.Src.Application.Features.Status.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class CourseMobileDto
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ThumbUrl { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public StatusDto? Status { get; set; }
    }
}
