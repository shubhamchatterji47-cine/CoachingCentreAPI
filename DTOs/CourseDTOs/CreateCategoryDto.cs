namespace CoachingAPI.DTOs.CourseDTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }
}
