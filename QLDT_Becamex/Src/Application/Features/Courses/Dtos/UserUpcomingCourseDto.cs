namespace QLDT_Becamex.Src.Application.Features.Courses.Dtos
{
    public class UserUpcomingCourseDto
    {
        public string? Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }
        public string? Optional { get; set; } = "tùy chọn";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
    }
}
