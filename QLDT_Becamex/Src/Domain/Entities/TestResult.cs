namespace QLDT_Becamex.Src.Domain.Entities
{
    // Đặt file này trong thư mục /Src/Domain/Entities/

    public class TestResult
    {
        public string Id { get; set; } = null!;

        // Khóa ngoại tới bài test đã được làm
        public int TestId { get; set; }
        public Test? Test { get; set; }

        // Khóa ngoại tới người dùng đã làm bài test
        public string UserId { get; set; } = null!;
        public ApplicationUser? User { get; set; }

        // Điểm số người dùng đạt được
        public float? Score { get; set; }

        // Cho biết người dùng đã vượt qua bài test hay chưa
        public bool IsPassed { get; set; } = false;

        public bool IsDone { get; set; } = false;

        public int CorrectAnswerCount { get; set; }
        public int IncorrectAnswerCount { get; set; }

        // Thời gian bắt đầu và kết thúc làm bài
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }

        // Danh sách các câu trả lời chi tiết của người dùng cho lượt làm bài này
        public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
}
