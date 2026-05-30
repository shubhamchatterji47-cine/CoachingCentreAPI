using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace CoachingAPI.Models
{
    public class Course
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(150)] public string Name { get; set; } = "";
        [MaxLength(500)] public string? Description { get; set; }
        [ForeignKey("Category")] public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        [Column(TypeName = "decimal(10,2)")] public decimal? Fee { get; set; }
        public int DurationMonths { get; set; }
        public string? Thumbnail { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Class> Classes { get; set; } = new List<Class>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
