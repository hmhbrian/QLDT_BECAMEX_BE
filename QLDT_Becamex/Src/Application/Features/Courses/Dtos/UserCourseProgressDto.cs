namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserCourseProgressDto
    {
        public string userId { get; set; } = null!;
        public string userName { get; set; } = null!;
        public float progressPercentage { get; set; }
    }
}