using System.Runtime.InteropServices;

namespace QLDT_Becamex.Src.Models
{
    public class Course
    {
        public string Id { get; set; } = null!;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Objecttives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sesstions { get; set; }
        public int? HoursPerSesstions { get; set; }
        public string? Optional { get; set; } = "tùy chọn";
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationSlosingDate { get; set; }

        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? StatusId { get; set; }
        public CourseSatus? Status { get; set; }
        public ICollection<CourseDepartment>? CourseDepartments { get; set; } = new List<CourseDepartment>();
        public ICollection<CoursePosition>? CoursePositions { get; set; } = new List<CoursePosition>();
        public ICollection<UserCourse>? UserCourses { get; set; } = new List<UserCourse>();

    }
}
