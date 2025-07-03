namespace QLDT_Becamex.Src.Application.Features.Questions.Dtos
{
    public class QuestionDto
    {
        public string? question_text { get; set; }
        public string? correct_option { get; set; }
        public int question_type { get; set; }
        public string? explanation { get; set; }
        public string? A { get; set; }
        public string? B { get; set; }
        public string? C { get; set; }
        public string? D { get; set; }
    }
}
