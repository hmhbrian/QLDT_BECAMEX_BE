namespace QLDT_Becamex.Src.Domain.Entities
{
    // Đặt file này trong thư mục /Src/Domain/Entities/
    public class UserAnswer
    {
        public int Id { get; set; }

        // Khóa ngoại tới phiếu kết quả tổng quan
        public string? TestResultId { get; set; }
        public TestResult? TestResult { get; set; }

        // Khóa ngoại tới câu hỏi đã được trả lời
        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        // Đáp án mà người dùng đã chọn (ví dụ: "A", "B", "C", hoặc "D")
        public string? SelectedOptions { get; set; }

        // Lưu lại đáp án này là đúng hay sai để tiện truy vấn sau này
        public bool IsCorrect { get; set; } = false;
    }
}
