namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserUpcomingCourseDto
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Objectives { get; set; }
        public string? ThumbUrl { get; set; }
        public bool? IsPrivate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }
        public string? Optional { get; set; } = "tùy chọn";
    }
}
