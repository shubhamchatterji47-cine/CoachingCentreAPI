using CoachingAPI.DTOs.ClassSubjectDTOs;

namespace CoachingAPI.DTOs.TeacherDTOs
{
    public class TeacherDashboardDto
    {
        public int TotalStudents { get; set; }
        public int TotalClasses { get; set; }
        public List<ClassDto> AssignedClasses { get; set; } = new();
    }
}
