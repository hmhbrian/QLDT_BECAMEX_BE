namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserCourseProgressDto
    {
        public string userId { get; set; } = null!;
        public string userName { get; set; } = null!;
        public float progressPercentage { get; set; }
    }

    public class LessonProgressDto
    {
        public string LessonId { get; set; } = null!;
        public string LessonName { get; set; } = null!;
        public float ProgressPercentage { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class TestScoreDto
    {
        public string TestId { get; set; } = null!;
        public string TestName { get; set; } = null!;
        public bool IsPassed { get; set; }
        public float? Score { get; set; }
        public DateTime? AttemptDate { get; set; }
    }

    public class DetailedUserCourseProgressDto : UserCourseProgressDto
    {
        public string courseId { get; set; } = null!;
        public string courseName { get; set; } = null!;
        public int Status { get; set; }
        public List<LessonProgressDto> LessonProgress { get; set; } = new List<LessonProgressDto>();
        public List<TestScoreDto> TestScore { get; set; } = new List<TestScoreDto>();
    }
}