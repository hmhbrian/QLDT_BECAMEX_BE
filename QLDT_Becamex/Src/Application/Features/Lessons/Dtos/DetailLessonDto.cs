using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Lessons.Dtos
{
    public class DetailLessonDto
    {
        public int Id { get; set; }
        public string? title { get; set; }
        public string? content_pdf { get; set; }
        public int Order { get; set; }
        public string? UserIdCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public string? UserNameEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
