using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestDto
    {
        public string? Id { get; set; }
        public string? course_id { get; set; }
        public string? title { get; set; }
        public float pass_threshold { get; set; }
        public int time_test { get; set; }
        public string? userId_created { get; set; }
        public string? userId_edited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Question>? Tests { get; set; } = new List<Question>();
    }
}