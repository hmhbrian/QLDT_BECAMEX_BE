namespace QLDT_Becamex.Src.Application.Features.Questions.Dtos
{
    public class QuestionDto
    {
        public int? Id { get; set; }
        public string? QuestionText { get; set; }
        public string? CorrectOption { get; set; }
        public int QuestionType { get; set; }
        public string? Explanation { get; set; }
        public int Position { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
    }
}
