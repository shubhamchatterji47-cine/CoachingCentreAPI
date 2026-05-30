using System.ComponentModel.DataAnnotations;
namespace CoachingAPI.DTOs.TeacherDTOs
{
    public class CreateTeacherDto
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = "";
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = "";
        [Required][MinLength(6)] public string Password { get; set; } = "";
        [Phone][MaxLength(20)] public string? PhoneNumber { get; set; }
        [MaxLength(200)] public string? Qualification { get; set; }
        [MaxLength(200)] public string? Specialization { get; set; }
        [Range(0, 50)] public int ExperienceYears { get; set; }
        [MaxLength(1000)] public string? Bio { get; set; }
    }
}