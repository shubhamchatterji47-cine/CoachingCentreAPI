using System.ComponentModel.DataAnnotations;
namespace CoachingAPI.DTOs.StudentDTOs
{
    public class CreateStudentDto
    {
        [Required][MaxLength(100)] public string FullName { get; set; } = "";
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = "";
        [Required][MinLength(6)] public string Password { get; set; } = "";
        [Phone][MaxLength(20)] public string? PhoneNumber { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a class")]
        public int ClassId { get; set; }
        [MaxLength(50)] public string? RollNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(100)] public string? ParentName { get; set; }
        [Phone][MaxLength(20)] public string? ParentPhone { get; set; }
        [MaxLength(300)] public string? Address { get; set; }
    }
}