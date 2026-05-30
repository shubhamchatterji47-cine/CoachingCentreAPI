using CoachingAPI.DTOs.CourseDTOs;

namespace CoachingAPI.DTOs.DashboardDTOs
{
    public class AdminDashboardDto
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalClasses { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
        public List<AnnouncementDto> RecentAnnouncements { get; set; } = new();
    }
}
