namespace CoachingAPI.DTOs.DashboardDTOs
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string? TargetRole { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
