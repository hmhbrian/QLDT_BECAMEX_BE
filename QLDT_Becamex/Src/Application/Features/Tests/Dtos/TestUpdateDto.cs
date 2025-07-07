using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestUpdateDto
    {
        public string Title { get; set; } = null!; // Bắt buộc
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
    }
}