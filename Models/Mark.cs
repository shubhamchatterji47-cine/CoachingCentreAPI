using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class Mark
    {
        [Key] public int Id { get; set; }
        [ForeignKey("Student")] public int StudentId { get; set; }
        public Student Student { get; set; } = null!;
        [ForeignKey("Subject")] public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        [Column(TypeName = "decimal(5,2)")] public decimal ObtainedMarks { get; set; }
        [MaxLength(50)] public string ExamType { get; set; } = "Test"; // Test, MidTerm, Final
        [MaxLength(200)] public string? Remarks { get; set; }
        public DateTime ExamDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("Teacher")] public int? EnteredByTeacherId { get; set; }
        public Teacher? EnteredByTeacher { get; set; }
    }
}
