using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.Models
{
    public class Announcement
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(200)] public string Title { get; set; } = "";
        [Required] public string Content { get; set; } = "";
        public string? TargetRole { get; set; } // null=all, Teacher, Student
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
