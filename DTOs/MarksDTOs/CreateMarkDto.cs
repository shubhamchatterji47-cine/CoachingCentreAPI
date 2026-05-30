using System.ComponentModel.DataAnnotations;
namespace CoachingAPI.DTOs.MarksDTOs
{
    public class CreateMarkDto
    {
        [Required][Range(1, int.MaxValue)] public int StudentId { get; set; }
        [Required][Range(1, int.MaxValue)] public int SubjectId { get; set; }
        [Required][Range(0, 1000)] public decimal ObtainedMarks { get; set; }
        [Required][MaxLength(50)] public string ExamType { get; set; } = "Test";
        [MaxLength(300)] public string? Remarks { get; set; }
        [Required] public DateTime ExamDate { get; set; } = DateTime.UtcNow;
    }
}