using System.ComponentModel.DataAnnotations;
namespace CoachingAPI.DTOs.CourseDTOs
{
    public class CreateCourseDto
    {
        [Required][MaxLength(150)] public string Name { get; set; } = "";
        [MaxLength(500)] public string? Description { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }
        [Range(0, 999999)] public decimal? Fee { get; set; }
        [Range(1, 60)] public int DurationMonths { get; set; }
    }
}