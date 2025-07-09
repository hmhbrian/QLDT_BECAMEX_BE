namespace QLDT_Becamex.Src.Domain.Entities
{
    public class LessonProgress
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public int? CurrentTimeSeconds { get; set; } // Thời gian hiện tại của người dùng trong bài học (tính bằng giây)
        public int? CurrentPage { get; set; } // Trang hiện tại của người dùng trong bài học
        public DateTime? LastUpdated { get; set; } // Thời gian cập nhật lần cuối của tiến độ bài học
    }
}
