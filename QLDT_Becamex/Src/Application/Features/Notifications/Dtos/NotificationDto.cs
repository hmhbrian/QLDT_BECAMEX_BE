namespace QLDT_Becamex.Src.Application.Features.Notifications.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }          
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? DataJson { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}
