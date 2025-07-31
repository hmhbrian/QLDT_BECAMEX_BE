
namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Certificates
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string? CourseId { get; set; }
        public virtual Course? Course { get; set; }
        public string? CertificateUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
