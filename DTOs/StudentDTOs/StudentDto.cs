namespace CoachingAPI.DTOs.StudentDTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; } = "";
        public string CourseName { get; set; } = "";
        public string? RollNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ParentName { get; set; }
        public string? ParentPhone { get; set; }
        public string? Address { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }
}
