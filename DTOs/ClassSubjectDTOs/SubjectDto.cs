namespace CoachingAPI.DTOs.ClassSubjectDTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
    }
}
