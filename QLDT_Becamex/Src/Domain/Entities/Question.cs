namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int? TestId { get; set; }
        public Test? Test { get; set; }
        public string? QuestionText { get; set; }
        public string? CorrectOption { get; set; }
        public int? QuestionType { get; set; }
        public string? Explanation { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
