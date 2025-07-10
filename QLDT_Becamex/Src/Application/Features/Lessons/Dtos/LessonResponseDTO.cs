namespace QLDT_Becamex.Src.Application.Features.Lessons.Dtos
{
    public class AllLessonDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }
        public string? Type { get; set; }
    }
    public class DetailLessonDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? FileUrl { get; set; }
        public string? Type { get; set; }
        public string? UserIdCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public string? UserIdEdited { get; set; }
        public string? UserNameEdited { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
