    using Microsoft.AspNet.Identity;

    namespace QLDT_Becamex.Src.Domain.Entities
    {
        public class Certificates
        {
            public int Id { get; set; }
            public string? UserId { get; set; }
            public ApplicationUser? User { get; set; }
            public string? CourseId { get; set; }
            public Course? Course { get; set; }
            public string? CertificateUrl { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
