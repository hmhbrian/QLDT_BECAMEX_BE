using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestCreateDto
    {
        public string course_id { get; set; } = null!; // Bắt buộc
        public string title { get; set; } = null!; // Bắt buộc
        public float pass_threshold { get; set; }
        public int time_test { get; set; }
        public string userId_created { get; set; } = null!; // Bắt buộc
        public ICollection<QuestionDto> Tests { get; set; } = new List<QuestionDto>();
    }
}