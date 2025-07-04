using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestUpdateDto
    {
        public string title { get; set; } = null!; // Bắt buộc
        public float pass_threshold { get; set; }
        public int time_test { get; set; }
        public ICollection<QuestionDto> Tests { get; set; } = new List<QuestionDto>();
    }
}