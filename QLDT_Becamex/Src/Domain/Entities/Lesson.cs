
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
        public string UrlPdf { get; set; } = null!;
        public string PublicIdUrlPdf { get; set; } = null!;
        public int Position { get; set; }
        public string? UserIdCreated { get; set; }
        public ApplicationUser? UserCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public ApplicationUser? UserEdited { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public void Create(string courseId, string userIdCreated, CreateLessonDto request, string urlPdf, string filePublicId)
        {
            Title = request.Title.ToLower().Trim();
            UrlPdf = urlPdf;
            PublicIdUrlPdf = filePublicId;
            CourseId = courseId;
            UserIdCreated = userIdCreated;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }

        public void Update(string courseId, string userIdEdited, UpdateLessonDto request, string urlPdf, string newFilePublicId)
        {
            if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != Title)
                Title = request.Title;

            if (!string.IsNullOrWhiteSpace(urlPdf) && urlPdf != UrlPdf)
                UrlPdf = urlPdf;

            if (!string.IsNullOrWhiteSpace(newFilePublicId) && PublicIdUrlPdf != newFilePublicId)
                PublicIdUrlPdf = newFilePublicId;


            if (courseId != CourseId)
                CourseId = courseId;

            UserIdEdited = userIdEdited;
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
