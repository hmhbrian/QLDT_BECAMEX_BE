using System.ComponentModel.DataAnnotations;

namespace QLDT_Becamex.Src.Application.Features.Questions.Dtos
{
    public class CreateQuestionDto
    {
        [Required]
        public string? QuestionText { get; set; }
        public string? CorrectOption { get; set; }
        public int? QuestionType { get; set; }
        public string? Explanation { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
    }
}
