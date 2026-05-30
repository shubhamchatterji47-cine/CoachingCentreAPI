namespace CoachingAPI.DTOs.DashboardDTOs
{
    public class CreateAnnouncementDto
    {
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string? TargetRole { get; set; }
    }
}
