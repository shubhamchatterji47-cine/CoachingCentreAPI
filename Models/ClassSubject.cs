using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class ClassSubject
    {
        [Key] public int Id { get; set; }
        [ForeignKey("Class")] public int ClassId { get; set; }
        public Class Class { get; set; } = null!;
        [ForeignKey("Subject")] public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
        [ForeignKey("Teacher")] public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;
    }
}
