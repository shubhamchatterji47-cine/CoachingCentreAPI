namespace CoachingAPI.Services
{
    public class PerformanceAnalyzer
    {
        public static string GetGrade(double percentage) => percentage switch
        {
            >= 90 => "A+",
            >= 80 => "A",
            >= 70 => "B+",
            >= 60 => "B",
            >= 50 => "C",
            >= 35 => "D",
            _ => "F"
        };

        public static string GetTrend(List<double> percentages)
        {
            if (percentages.Count < 2) return "Insufficient Data";
            var recent = percentages.TakeLast(2).ToList();
            var diff = recent[1] - recent[0];
            return diff > 5 ? "Improving" : diff < -5 ? "Declining" : "Stable";
        }

        public static List<string> GenerateImprovementTips(double overallPct, List<string> weakSubjects, string trend)
        {
            var tips = new List<string>();

            if (overallPct < 50)
            {
                tips.Add("📅 Create a structured daily study timetable and stick to it consistently.");
                tips.Add("🔁 Review class notes within 24 hours of each lecture to reinforce learning.");
                tips.Add("📞 Don't hesitate to ask your teacher for help on difficult topics.");
            }
            else if (overallPct < 70)
            {
                tips.Add("🎯 Focus extra time on weak subjects while maintaining strong ones.");
                tips.Add("📝 Practice previous exam papers to get familiar with question patterns.");
                tips.Add("👥 Form a study group with classmates for peer learning.");
            }
            else if (overallPct < 85)
            {
                tips.Add("🚀 You're doing well! Push for excellence by solving advanced problems.");
                tips.Add("⏱️ Work on speed and accuracy with timed practice sessions.");
                tips.Add("📖 Explore reference books beyond the standard curriculum.");
            }
            else
            {
                tips.Add("🏆 Outstanding performance! Consider mentoring fellow students.");
                tips.Add("🎓 Explore competitive exam preparation to challenge yourself further.");
                tips.Add("💡 Engage with conceptual problems and real-world applications.");
            }

            if (weakSubjects.Any())
                tips.Add($"⚠️ Give extra attention to: {string.Join(", ", weakSubjects)}. Allocate at least 1 extra hour daily to these.");

            if (trend == "Declining")
                tips.Add("📉 Your scores are declining. Identify the cause — consider discussing with your teacher immediately.");
            else if (trend == "Improving")
                tips.Add("📈 Great momentum! Keep up the consistent effort you've been putting in.");

            tips.Add("😴 Ensure 7-8 hours of sleep; it significantly improves memory retention and focus.");
            tips.Add("🏃 Stay physically active — 30 minutes of exercise daily boosts concentration.");

            return tips;
        }
    }
}
