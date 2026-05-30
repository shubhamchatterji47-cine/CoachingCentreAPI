using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class Attendance
    {
        [Key] public int Id { get; set; }
        [ForeignKey("Student")] public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.UtcNow.Date;
        public bool IsPresent { get; set; } = true;
        [MaxLength(200)] public string? Notes { get; set; }
    }
}
