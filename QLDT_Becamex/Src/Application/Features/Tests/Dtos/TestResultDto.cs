using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class UserAnswerAndCorrectAnswerDto
    {
        public Question? Question { get; set; }
        // đáp án user chọn
        public string? SelectedOptions { get; set; }
        // đáp án đúng
        public string? CorrectAnswer { get; set; }

        // Lưu lại đáp án này là đúng hay sai để tiện truy vấn sau này
        public bool IsCorrect { get; set; } = false;
    }
    public class TestResultDto
    {
        // Khóa ngoại tới bài test đã được làm
        public Test? Test { get; set; }

        // Điểm số người dùng đạt được
        public float? Score { get; set; }

        // Cho biết người dùng đã vượt qua bài test hay chưa
        public bool IsPassed { get; set; } = false;

        // Thời gian bắt đầu và kết thúc làm bài
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
    public class DetailTestResultDto : TestResultDto
    {
        public ICollection<UserAnswerAndCorrectAnswerDto> UserAnswers { get; set; } = new List<UserAnswerAndCorrectAnswerDto>();
    }
}
