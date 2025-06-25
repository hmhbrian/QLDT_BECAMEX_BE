namespace QLDT_Becamex.Src.Domain.Entities
{
    public class UserCourse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public DateTime? AssignedAt { get; set; }
        public bool IsMandatory { get; set; } = false;// Đánh dấu là bắt buộc
        public string? Status { get; set; } = null;
        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
    }
}
