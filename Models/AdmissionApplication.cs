using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoachingAPI.Models
{
    public class AdmissionApplication
    {
        [Key] public int Id { get; set; }

        [Required, MaxLength(100)] public string FullName { get; set; } = "";
        [Required, MaxLength(150)] public string Email { get; set; } = "";
        [Required, MaxLength(20)] public string PhoneNumber { get; set; } = "";
        [MaxLength(20)] public string? ParentPhone { get; set; }
        [MaxLength(100)] public string? ParentName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(300)] public string? Address { get; set; }
        [MaxLength(200)] public string? PreviousSchool { get; set; }
        [MaxLength(100)] public string? LastClassPassed { get; set; }

        // Which course they're interested in
        [ForeignKey("Course")]
        public int? CourseId { get; set; }
        public Course? Course { get; set; }
        [MaxLength(200)] public string? CourseInterest { get; set; } // free text if no course

        [MaxLength(1000)] public string? Message { get; set; }

        // Status: Pending, Reviewed, Approved, Rejected
        [MaxLength(20)] public string Status { get; set; } = "Pending";
        [MaxLength(500)] public string? AdminNotes { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
    }
}