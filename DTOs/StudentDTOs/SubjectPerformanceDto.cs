using CoachingAPI.DTOs.MarksDTOs;

namespace CoachingAPI.DTOs.StudentDTOs
{
    public class SubjectPerformanceDto
    {
        public string SubjectName { get; set; } = "";
        public List<MarkDto> Marks { get; set; } = new();
        public double AveragePercentage { get; set; }
        public string Grade { get; set; } = "";
        public string Trend { get; set; } = "";
        public int MaxMarks { get; set; }
    }
}
