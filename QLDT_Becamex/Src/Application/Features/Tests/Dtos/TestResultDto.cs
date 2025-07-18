

using QLDT_Becamex.Src.Application.Features.Users.Dtos;

namespace QLDT_Becamex.Src.Application.Features.Tests.Dtos
{
    public class TestResultDto
    {
        public string Id { get; set; } = null!;

        // Khóa ngoại tới bài test đã được làm
        public int TestId { get; set; }

        // Khóa ngoại tới người dùng đã làm bài test
        public ByUser User { get; set; } = null!;

        // Điểm số người dùng đạt được
        public float? Score { get; set; }

        // Cho biết người dùng đã vượt qua bài test hay chưa
        public bool IsPassed { get; set; } = false;

        // Thời gian bắt đầu và kết thúc làm bài
        public DateTime? StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
    }
}
