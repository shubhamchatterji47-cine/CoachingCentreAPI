namespace CoachingAPI.DTOs.ClassSubjectDTOs
{
    public class CreateSubjectDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public int MaxMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;
    }
}
