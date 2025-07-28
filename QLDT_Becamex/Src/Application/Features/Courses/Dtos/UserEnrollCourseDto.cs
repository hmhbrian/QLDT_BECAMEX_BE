namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserEnrollCourseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ThumbUrl { get; set; }
        public float progressPercentange { get; set; }
    }
    public class UserEnrollCompletedCourseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ThumbUrl { get; set; }
    }
}
