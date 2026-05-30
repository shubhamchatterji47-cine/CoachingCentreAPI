using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace CoachingAPI.Models
{
    public class Student
    {
        [Key] public int Id { get; set; }
        [ForeignKey("User")] public int UserId { get; set; }
        public User User { get; set; } = null!;
        [ForeignKey("Class")] public int ClassId { get; set; }
        public Class Class { get; set; } = null!;
        [MaxLength(50)] public string? RollNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(200)] public string? ParentName { get; set; }
        [MaxLength(20)] public string? ParentPhone { get; set; }
        [MaxLength(300)] public string? Address { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
