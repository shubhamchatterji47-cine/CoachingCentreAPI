using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.Models
{
    public class ContactMessage
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [Required, MaxLength(150)] public string Email { get; set; } = "";
        [MaxLength(20)] public string? Phone { get; set; }
        [MaxLength(100)] public string? Subject { get; set; }
        [Required] public string Message { get; set; } = "";
        public bool IsRead { get; set; } = false;
        public string Status { get; set; } = "New"; // New, Read, Replied
        public string? AdminReply { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}