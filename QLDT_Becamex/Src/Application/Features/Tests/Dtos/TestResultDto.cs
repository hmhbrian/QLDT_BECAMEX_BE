using QLDT_Becamex.Src.Domain.Entities;


using QLDT_Becamex.Src.Application.Features.Users.Dtos;
using Newtonsoft.Json;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class UserAnswerAndCorrectAnswerDto
    {
        public Question? Question { get; set; }
        public string? SelectedOptions { get; set; }
        public string? CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]

    public class TestResultDto
    {
        public string? Id { get; set; }
        public TestSummaryDto? Test { get; set; }
        public float? Score { get; set; }
        public UserSumaryDto? User { get; set; }
        public bool IsPassed { get; set; } = false;
        public bool IsFinish { get; set; } = false;
        public int CorrectAnswerCount { get; set; }
        public int IncorrectAnswerCount { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }

    public class DetailTestResultDto : TestResultDto
    {
        public ICollection<UserAnswerAndCorrectAnswerDto> UserAnswers { get; set; } = new List<UserAnswerAndCorrectAnswerDto>();
    }


}
