using Microsoft.AspNetCore.Http.HttpResults;
using QLDT_Becamex.Src.Application.Features.LessonProgresses.Dtos;
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class LessonProgress
    {
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
        public bool IsCompleted { get; set; } = false; // Trạng thái hoàn thành của bài học
        public int? CurrentTimeSeconds { get; set; } // Thời gian hiện tại của người dùng trong bài học (tính bằng giây)
        public int? CurrentPage { get; set; } // Trang hiện tại của người dùng trong bài học
        public DateTime? LastUpdated { get; set; } // Thời gian cập nhật lần cuối của tiến độ bài học

        public void Create( string userId, UpsertLessonProgressDto request, bool isCompleted)
        {
            UserId = userId;
            LessonId = request.LessonId;
            if (request.CurrentPage > 0)
                CurrentPage = request.CurrentPage; // lưu tổng số trang của PDF
            else
                CurrentTimeSeconds = request.CurrentTimeSecond;//Lưu tổng thời gian của bài học
            IsCompleted = isCompleted;
            LastUpdated = DateTime.Now;
        }

        public void Update(UpsertLessonProgressDto request, bool isCompleted)
        {
            if(request.LessonId != LessonId)
                LessonId = request.LessonId; // Cập nhật LessonId nếu có thay đổi

            if (request.CurrentPage > 0)
                CurrentPage = request.CurrentPage; // lưu tổng số trang của PDF
            else
                CurrentTimeSeconds = request.CurrentTimeSecond;//Lưu tổng thời gian của bài học
            IsCompleted = isCompleted;
            LastUpdated = DateTime.Now;
        }
    }
}
