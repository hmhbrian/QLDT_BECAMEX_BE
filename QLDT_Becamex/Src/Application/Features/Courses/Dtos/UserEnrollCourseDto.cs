namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserEnrollCourseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int LessonCompletedCount { get; set; }
        public int TotalLessonCount { get; set; }
        public int TestCompletedCount { get; set; }
        public int TotalTestCount { get; set; }
        public int Status { get; set; }
        public string? ThumbUrl { get; set; }
        public float progressPercentage { get; set; }
    }
    public class UserEnrollCompletedCourseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ThumbUrl { get; set; }
    }
}
