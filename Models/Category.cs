using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.Models
{
    public class Category
    {
        [Key] public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [MaxLength(300)] public string? Description { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
