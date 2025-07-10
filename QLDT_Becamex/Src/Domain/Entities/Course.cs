

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class Course
    {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Objectives { get; set; }
        public string? ThumbUrl { get; set; }
        public string? Format { get; set; }
        public int? Sessions { get; set; }
        public int? HoursPerSessions { get; set; }
        public string? Optional { get; set; } = "tùy chọn";
        public int? MaxParticipant { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationClosingDate { get; set; }
        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int? StatusId { get; set; }
        public CourseStatus? Status { get; set; }
        public int? CategoryId { get; set; }
        public CourseCategory? Category { get; set; }
        public int? LecturerId { get; set; }
        public Lecturer? Lecturer { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CreateById { get; set; }
        public ApplicationUser? CreateBy { get; set; }
        public string? UpdateById { get; set; }
        public ApplicationUser? UpdateBy { get; set; }
        public ICollection<CourseDepartment>? CourseDepartments { get; set; } = new List<CourseDepartment>();
        public ICollection<CoursePosition>? CoursePositions { get; set; } = new List<CoursePosition>();
        public ICollection<UserCourse>? UserCourses { get; set; } = new List<UserCourse>();
        public ICollection<CourseAttachedFile>? AttachedFiles { get; set; }
        public ICollection<Lesson>? Lessons { get; set; } = new List<Lesson>();
        public ICollection<Test>? Tests { get; set; } = new List<Test>();

    }
}
