

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos
{
    public class CourseAttachedFileDto
    {
        public int Id { get; set; }
        public string? CourseId { get; set; } = null;
        public string? Title { get; set; } = null;

        public string? Type { get; set; } = null;

        public string? Link { get; set; } = null;
        public string? PublicIdUrlPdf { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }


    }
}
