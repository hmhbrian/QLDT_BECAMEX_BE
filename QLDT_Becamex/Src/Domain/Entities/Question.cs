namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public int? test_id { get; set; }
        public Test? Test { get; set; }
        public string? question_text { get; set; }
        public string? correct_option { get; set; }
        public int question_type { get; set; }
        public string? explanation { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
