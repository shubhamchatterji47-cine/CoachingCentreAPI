namespace CoachingAPI.DTOs.CourseDTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal? Fee { get; set; }
        public int DurationMonths { get; set; }
        public string? Thumbnail { get; set; }
        public bool IsActive { get; set; }
        public int ClassCount { get; set; }
    }
}
