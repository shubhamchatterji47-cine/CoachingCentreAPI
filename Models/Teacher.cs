using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class Teacher
    {
        [Key] public int Id { get; set; }
        [ForeignKey("User")] public int UserId { get; set; }
        public User User { get; set; } = null!;
        [MaxLength(200)] public string? Qualification { get; set; }
        [MaxLength(200)] public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }

        // Navigation
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
