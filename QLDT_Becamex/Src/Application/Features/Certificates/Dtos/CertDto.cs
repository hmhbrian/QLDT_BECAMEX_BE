using QLDT_Becamex.Src.Application.Features.Courses.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Certificates.Dtos
{

    public class CertDetailDto
    {
        public int Id { get; set; }
        public UserSumaryDto? User { get; set; }
        public CourseSumary? Course { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class CertListDto
    {
        public int Id { get; set; }
        public UserSumaryDto? User { get; set; }
        public CourseSumary? Course { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
