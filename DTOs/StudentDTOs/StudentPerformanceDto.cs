namespace CoachingAPI.DTOs.StudentDTOs
{
    public class StudentPerformanceDto
    {
        public StudentDto Student { get; set; } = null!;
        public List<SubjectPerformanceDto> SubjectPerformances { get; set; } = new();
        public double OverallPercentage { get; set; }
        public string OverallGrade { get; set; } = "";
        public string PerformanceTrend { get; set; } = ""; // Improving, Declining, Stable
        public List<string> ImprovementTips { get; set; } = new();
        public double AttendancePercentage { get; set; }
    }
}
