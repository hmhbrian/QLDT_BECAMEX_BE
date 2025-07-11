

namespace QLDT_Becamex.Src.Application.Features.CourseAttachedFile.Dtos
{
    public class CourseAttachedFileDto
    {
        public int Id { get; set; }
        public string? CourseId { get; set; }
        public string? Title { get; set; }

        public string? Type { get; set; }

        public string? Link { get; set; }
        public string? PublicIdUrlPdf { get; set; }


        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }


    }
}
