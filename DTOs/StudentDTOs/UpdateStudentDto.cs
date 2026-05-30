using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.DTOs.StudentDTOs
{
    public class UpdateStudentDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

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