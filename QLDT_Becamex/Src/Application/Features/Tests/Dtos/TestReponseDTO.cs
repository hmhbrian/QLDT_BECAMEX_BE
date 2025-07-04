using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class AllTestDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
        public int CountQuestion { get; set; }
    }

    public class DetailTestDto
    {
        public string? Id { get; set; }
        public string? CourseId { get; set; }
        public string? Title { get; set; }
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
        public string? UserIdCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<QuestionDto>? Questions { get; set; } = new List<QuestionDto>();
    }
}
