using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestReponseDTO
    {
        public class AllTestDto
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public float PassThreshold { get; set; }
            public int TimeTest { get; set; }
            public int CountQuestion { get; set; }
        }
    }
}
