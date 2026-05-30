namespace CoachingAPI.DTOs.ClassSubjectDTOs
{
    public class ClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public string? Schedule { get; set; }
        public string? Timing { get; set; }
        public int MaxCapacity { get; set; }
        public int EnrolledCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
