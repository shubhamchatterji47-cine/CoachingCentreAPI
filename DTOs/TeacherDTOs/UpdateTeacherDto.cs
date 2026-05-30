namespace CoachingAPI.DTOs.TeacherDTOs
{
    public class UpdateTeacherDto
    {
        public string FullName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? Qualification { get; set; }
        public string? Specialization { get; set; }
        public int ExperienceYears { get; set; }
        public string? Bio { get; set; }
    }
}