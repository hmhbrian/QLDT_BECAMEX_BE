using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestCreateDto
    {
        public string Title { get; set; } = null!; // Bắt buộc
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
        public ICollection<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
}