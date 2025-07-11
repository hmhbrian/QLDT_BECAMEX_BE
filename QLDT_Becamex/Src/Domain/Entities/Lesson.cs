
using Castle.Components.DictionaryAdapter.Xml;
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
        public string? FileUrl { get; set; }
        public string? PublicIdUrlPdf { get; set; }
        public int? TotalDurationSeconds { get; set; } // Tổng thời gian của bài học (tính bằng giây)
        public int? TotalPages { get; set; } // Tổng số trang của tài liệu PDF
        public int Position { get; set; }
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();

        public void Create(string courseId, string userIdCreated, CreateLessonDto request, string urlPdf, string filePublicId, int position,int typeId, int totalDurations, int totalPages)
        {
            Title = request.Title.ToLower().Trim();
            FileUrl = urlPdf;
            PublicIdUrlPdf = filePublicId;
            CourseId = courseId;
            Position = position;
            CreatedById = userIdCreated;

            if (totalDurations > 0)
                TotalDurationSeconds = totalDurations; // Lưu tổng thời gian của bài học
            else
                TotalPages = totalPages;//lưu tổng số trang của PDF
            TypeDocId = typeId; 
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void Update(string courseId, string userIdEdited, UpdateLessonDto request, string urlPdf, string newFilePublicId, int typeId, int totalDurations, int totalPages)
        {
            if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != Title)
                Title = request.Title;

            if (!string.IsNullOrWhiteSpace(urlPdf) && urlPdf != FileUrl)
                FileUrl = urlPdf;

            // Kiểm tra và xóa PublicIdUrlPdf nếu newFilePublicId là null hoặc TypeDocId là 2
            if (newFilePublicId == null || typeId == 2)
            {
                PublicIdUrlPdf = null!; // Xóa dữ liệu cũ của PublicIdUrlPdf
            }
            else if (!string.IsNullOrWhiteSpace(newFilePublicId) && PublicIdUrlPdf != newFilePublicId)
            {
                PublicIdUrlPdf = newFilePublicId; // Cập nhật nếu khác và không null
            }


            if (totalDurations > 0)
            {
                TotalDurationSeconds = totalDurations; // Lưu tổng thời gian của bài học
                TotalPages = null; // Đặt TotalPages là null nếu có tổng thời gian
            }
            else
            {
                TotalPages = totalPages;//lưu tổng số trang của PDF
                TotalDurationSeconds = null; // Đặt TotalDurationSeconds là null nếu có tổng số trang
            }

            TypeDocId = typeId;

            if (courseId != CourseId)
                CourseId = courseId;

            UpdatedById = userIdEdited;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
