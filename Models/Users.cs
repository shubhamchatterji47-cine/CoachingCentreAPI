using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string FullName { get; set; } = "";
        [Required, MaxLength(150)] public string Email { get; set; } = "";
        [Required] public string PasswordHash { get; set; } = "";
        [Required] public string Role { get; set; } = "Student"; // Admin, Teacher, Student
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }

        // Navigation
        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
    }
}
