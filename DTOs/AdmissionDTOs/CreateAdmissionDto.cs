using System.ComponentModel.DataAnnotations;

namespace CoachingAPI.DTOs.AdmissionDTOs
{
    public class CreateAdmissionDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)] public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress][MaxLength(150)] public string Email { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required")]
        [Phone][MaxLength(20)] public string PhoneNumber { get; set; } = "";

        [MaxLength(100)] public string? ParentName { get; set; }
        [MaxLength(20)] public string? ParentPhone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(300)] public string? Address { get; set; }
        [MaxLength(200)] public string? PreviousSchool { get; set; }
        [MaxLength(100)] public string? LastClassPassed { get; set; }
        public int? CourseId { get; set; }
        [MaxLength(200)] public string? CourseInterest { get; set; }
        [MaxLength(1000)] public string? Message { get; set; }
    }
}
