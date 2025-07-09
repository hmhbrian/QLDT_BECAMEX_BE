
using QLDT_Becamex.Src.Application.Features.Lessons.Dtos;
using QLDT_Becamex.Src.Shared.Helpers;
namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        public string CourseId { get; set; } = null!;
        public Course Course { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int TypeDocId { get; set; } = 1; // Mặc định là PDF
        public TypeDocument TypeDoc { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public string PublicIdUrlPdf { get; set; } = null!;
        public int? TotalDurationSeconds { get; set; } // Tổng thời gian của bài học (tính bằng giây)
        public int? TotalPages { get; set; } // Tổng số trang của tài liệu PDF
        public int Position { get; set; }
        public string? UserIdCreated { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<LessonProgress>? LessonProgress { get; set; } = new List<LessonProgress>();

        public void Create(string courseId, string userIdCreated, CreateLessonDto request, string urlPdf, string filePublicId, int position)
        {
            Title = request.Title.ToLower().Trim();
            FileUrl = urlPdf;
            PublicIdUrlPdf = filePublicId;
            CourseId = courseId;
            Position = position;
            UserIdCreated = userIdCreated;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void Update(string courseId, string userIdEdited, UpdateLessonDto request, string fileUrl, string newFilePublicId)
        {
            if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != Title)
                Title = request.Title;

            if (!string.IsNullOrWhiteSpace(fileUrl) && fileUrl != FileUrl)
                FileUrl = fileUrl;

            if (!string.IsNullOrWhiteSpace(newFilePublicId) && PublicIdUrlPdf != newFilePublicId)
                PublicIdUrlPdf = newFilePublicId;


            if (courseId != CourseId)
                CourseId = courseId;

            UserIdEdited = userIdEdited;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
