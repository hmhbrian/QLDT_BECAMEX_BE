using Microsoft.AspNet.Identity;

namespace QLDT_Becamex.Src.Domain.Entities
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string? UserId { get; set; } 
        public int MessageId { get; set; }
        public DateTime SentAt { get; set; }// thời điểm fan-out cho user
        public bool IsRead { get; set; } // default false
        public DateTime? ReadAt { get; set; }
        public bool IsHidden { get; set; } // nếu user ẩn khỏi inbox
        public Messages? Message { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
