namespace QLDT_Becamex.Src.Domain.Entities
{
    public class UserCourse
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public int Optional { get; set; } = 0; // 0 = tùy chọn && 1 = bắt buộc
        public DateTime? AssignedAt { get; set; }
        public bool IsMandatory { get; set; } = false;// Đánh dấu là bắt buộc
        public int Status { get; set; }
        public float PercentComplete { get; set; }
        public ApplicationUser? User { get; set; }
        public Course? Course { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
