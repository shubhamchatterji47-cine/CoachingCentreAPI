using System.ComponentModel.DataAnnotations;
namespace CoachingAPI.DTOs.ClassSubjectDTOs
{
    public class CreateClassDto
    {
        [Required][MaxLength(100)] public string Name { get; set; } = "";
        [MaxLength(300)] public string? Description { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a course")]
        public int CourseId { get; set; }
        [MaxLength(100)] public string? Schedule { get; set; }
        [MaxLength(100)] public string? Timing { get; set; }
        [Range(1, 500)] public int MaxCapacity { get; set; } = 30;
        [Required] public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}