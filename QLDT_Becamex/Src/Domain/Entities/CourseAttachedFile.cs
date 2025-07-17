

using QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class CourseAttachedFile
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int TypeDocId { get; set; } = 1; // Mặc định là PDF
        public TypeDocument TypeDoc { get; set; } = null!;
        public string? Link { get; set; }
        public string? PublicIdUrlPdf { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser UserCreated { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedTime { get; set; } = DateTime.Now;

        public void Create(string courseId, string userIdCreated, string title, string link, string filePublicId, int typeId)
        {
            Title = title.ToLower().Trim();
            Link = link;
            PublicIdUrlPdf = filePublicId;
            CourseId = courseId;
            UserId = userIdCreated;
            TypeDocId = typeId;
            CreatedAt = DateTime.Now;
            ModifiedTime = DateTime.Now;
        }
    }
}