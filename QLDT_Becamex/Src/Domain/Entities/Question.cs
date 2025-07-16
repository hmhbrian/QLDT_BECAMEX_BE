using QLDT_Becamex.Src.Application.Features.Questions.Dtos;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public Test? Test { get; set; }
        public int Position { get; set; }
        public string? QuestionText { get; set; }
        public string? CorrectOption { get; set; }
        public int? QuestionType { get; set; }
        public string? Explanation { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
        public string? CreateById { get; set; }
        public ApplicationUser? CreateBy { get; set; }
        public string? UpdateById { get; set; }
        public ApplicationUser? UpdateBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public void Create(int testId, CreateQuestionDto request, int position)
        {
            TestId = testId;
            QuestionText = request.QuestionText;
            CorrectOption = request.CorrectOption;
            QuestionType = request.QuestionType;
            Explanation = request.Explanation;
            A = request.A;
            B = request.B;
            C = request.C;
            D = request.D;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Position = position;
        }

        public void Update(UpdateQuestionDto request)
        {
            // Cập nhật các giá trị nếu các tham số không null
            if (!string.IsNullOrEmpty(request.QuestionText)) QuestionText = request.QuestionText;
            if (!string.IsNullOrEmpty(request.CorrectOption)) CorrectOption = request.CorrectOption;
            if (request.QuestionType.HasValue) QuestionType = request.QuestionType;
            if (!string.IsNullOrEmpty(request.Explanation)) Explanation = request.Explanation;
            if (!string.IsNullOrEmpty(request.A)) A = request.A;
            if (!string.IsNullOrEmpty(request.B)) B = request.B;
            if (!string.IsNullOrEmpty(request.C)) C = request.C;
            if (!string.IsNullOrEmpty(request.D)) D = request.D;
            // Cập nhật thời gian sửa đổi
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePosition(int position)
        {
            Position = position;
            UpdatedAt = DateTime.UtcNow; // Cập nhật thời gian sửa đổi

        }
    }
}
