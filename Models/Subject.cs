using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class Subject
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [MaxLength(300)] public string? Description { get; set; }
        [ForeignKey("Course")] public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public int MaxMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;

        // Navigation
        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
        public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    }
}
