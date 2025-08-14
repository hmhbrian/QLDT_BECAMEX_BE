using System.Collections;
using Newtonsoft.Json;
using QLDT_Becamex.Src.Application.Features.Questions.Dtos;
using QLDT_Becamex.Src.Application.Features.Users.Dtos;
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
        public bool IsDone { get; set; }
        public float Score { get; set; }
        public bool IsPassed { get; set; }
        public UserSumaryDto? CreatedBy { get; set; }
        public UserSumaryDto? UpdatedBy { get; set; }
    }

    public class DetailTestDto
    {
        public int? Id { get; set; }
        public string? CourseId { get; set; }
        public string? Title { get; set; }
        public float PassThreshold { get; set; }
        public int TimeTest { get; set; }
        public bool IsDone { get; set; }
        public UserSumaryDto? CreatedBy { get; set; }
        public UserSumaryDto? UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<QuestionDto>? Questions { get; set; } = new List<QuestionDto>();
    }

    public class TestCompletedDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
    }
    public class QuestionNoAnswerDto
    {
        public int? Id { get; set; }
        public string? QuestionText { get; set; }
        public int QuestionType { get; set; }
        public int Position { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
    }
}
