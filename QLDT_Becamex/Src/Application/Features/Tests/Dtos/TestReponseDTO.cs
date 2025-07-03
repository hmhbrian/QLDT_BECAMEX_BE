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
        public string? Course_id { get; set; }
        public string? Title { get; set; }
        public float Pass_threshold { get; set; }
        public int Time_test { get; set; }
        public string? UserId_created { get; set; }
        public string? UserId_edited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Questions { get; set; } = new List<Question>();
    }
}
