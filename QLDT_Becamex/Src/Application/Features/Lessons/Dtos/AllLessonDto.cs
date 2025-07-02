using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Dtos
{
    public class AllLessonDto
    {
        public int Id { get; set; }
        public string? title { get; set; }
        public string? content_pdf { get; set; }
        public int Order { get; set; }
    }
}
