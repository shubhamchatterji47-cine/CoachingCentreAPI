using CoachingAPI.Data;
using CoachingAPI.DTOs;
using CoachingAPI.DTOs.MarksDTOs;
using CoachingAPI.DTOs.StudentDTOs;
using CoachingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,Teacher,Admin")]
    public class StudentController : ControllerBase
    {
        private readonly AppDbContext _db;
        public StudentController(AppDbContext db) { _db = db; }

        private int GetStudentId() =>
            int.Parse(User.FindFirst("RoleEntityId")?.Value ?? "0");

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var studentId = GetStudentId();
            var student = await _db.Students
                .Include(s => s.User).Include(s => s.Class).ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null) return NotFound();

            return Ok(new StudentDto
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.User.FullName,
                Email = student.User.Email,
                PhoneNumber = student.User.PhoneNumber,
                ClassId = student.ClassId,
                ClassName = student.Class.Name,
                CourseName = student.Class.Course.Name,
                RollNumber = student.RollNumber,
                DateOfBirth = student.DateOfBirth,
                ParentName = student.ParentName,
                ParentPhone = student.ParentPhone,
                Address = student.Address,
                EnrollmentDate = student.EnrollmentDate,
                IsActive = student.User.IsActive
            });
        }

        [HttpGet("profile/{studentId}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GetProfileById(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.User).Include(s => s.Class).ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            return Ok(new StudentDto
            {
                Id = student.Id,
                UserId = student.UserId,
                FullName = student.User.FullName,
                Email = student.User.Email,
                PhoneNumber = student.User.PhoneNumber,
                ClassId = student.ClassId,
                ClassName = student.Class.Name,
                CourseName = student.Class.Course.Name,
                RollNumber = student.RollNumber,
                DateOfBirth = student.DateOfBirth,
                ParentName = student.ParentName,
                EnrollmentDate = student.EnrollmentDate,
                IsActive = student.User.IsActive
            });
        }

        [HttpGet("performance")]
        public async Task<IActionResult> GetMyPerformance()
        {
            var studentId = GetStudentId();
            return await BuildPerformance(studentId);
        }

        [HttpGet("performance/{studentId}")]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> GetPerformanceById(int studentId)
        {
            return await BuildPerformance(studentId);
        }

        private async Task<IActionResult> BuildPerformance(int studentId)
        {
            var student = await _db.Students
                .Include(s => s.User).Include(s => s.Class).ThenInclude(c => c.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null) return NotFound();

            // Get subjects for student's course
            var subjects = await _db.Subjects
                .Where(sub => sub.CourseId == student.Class.CourseId).ToListAsync();

            // Get all marks
            var marks = await _db.Marks
                .Include(m => m.Subject)
                .Where(m => m.StudentId == studentId)
                .OrderBy(m => m.ExamDate)
                .ToListAsync();

            // Build subject performances
            var subjectPerformances = subjects.Select(sub =>
            {
                var subMarks = marks.Where(m => m.SubjectId == sub.Id).ToList();
                var percentages = subMarks.Select(m => (double)m.ObtainedMarks / sub.MaxMarks * 100).ToList();
                var avg = percentages.Any() ? percentages.Average() : 0;
                return new SubjectPerformanceDto
                {
                    SubjectName = sub.Name,
                    MaxMarks = sub.MaxMarks,
                    Marks = subMarks.Select(m => new MarkDto
                    {
                        Id = m.Id,
                        StudentId = m.StudentId,
                        SubjectId = m.SubjectId,
                        SubjectName = sub.Name,
                        ObtainedMarks = m.ObtainedMarks,
                        MaxMarks = sub.MaxMarks,
                        ExamType = m.ExamType,
                        Remarks = m.Remarks,
                        ExamDate = m.ExamDate,
                        Percentage = Math.Round((double)m.ObtainedMarks / sub.MaxMarks * 100, 2),
                        Grade = PerformanceAnalyzer.GetGrade((double)m.ObtainedMarks / sub.MaxMarks * 100)
                    }).ToList(),
                    AveragePercentage = Math.Round(avg, 2),
                    Grade = PerformanceAnalyzer.GetGrade(avg),
                    Trend = PerformanceAnalyzer.GetTrend(percentages)
                };
            }).ToList();

            var allPercentages = subjectPerformances
                .Where(sp => sp.Marks.Any())
                .Select(sp => sp.AveragePercentage).ToList();

            var overallPct = allPercentages.Any() ? allPercentages.Average() : 0;

            var weakSubjects = subjectPerformances
                .Where(sp => sp.AveragePercentage < 50 && sp.Marks.Any())
                .Select(sp => sp.SubjectName).ToList();

            var allPctList = marks.Select(m => (double)m.ObtainedMarks / m.Subject.MaxMarks * 100).ToList();
            var trend = PerformanceAnalyzer.GetTrend(allPctList);

            // Attendance
            var totalDays = await _db.Attendances.CountAsync(a => a.StudentId == studentId);
            var presentDays = await _db.Attendances.CountAsync(a => a.StudentId == studentId && a.IsPresent);
            var attendancePct = totalDays > 0 ? (double)presentDays / totalDays * 100 : 100;

            return Ok(new StudentPerformanceDto
            {
                Student = new StudentDto
                {
                    Id = student.Id,
                    FullName = student.User.FullName,
                    Email = student.User.Email,
                    ClassName = student.Class.Name,
                    CourseName = student.Class.Course.Name,
                    RollNumber = student.RollNumber,
                    EnrollmentDate = student.EnrollmentDate
                },
                SubjectPerformances = subjectPerformances,
                OverallPercentage = Math.Round(overallPct, 2),
                OverallGrade = PerformanceAnalyzer.GetGrade(overallPct),
                PerformanceTrend = trend,
                ImprovementTips = PerformanceAnalyzer.GenerateImprovementTips(overallPct, weakSubjects, trend),
                AttendancePercentage = Math.Round(attendancePct, 2)
            });
        }

        [HttpGet("marks")]
        public async Task<IActionResult> GetMyMarks([FromQuery] string? examType)
        {
            var studentId = GetStudentId();
            var query = _db.Marks.Include(m => m.Subject).Where(m => m.StudentId == studentId);
            if (!string.IsNullOrEmpty(examType)) query = query.Where(m => m.ExamType == examType);

            return Ok(await query.OrderByDescending(m => m.ExamDate).Select(m => new MarkDto
            {
                Id = m.Id,
                StudentId = m.StudentId,
                SubjectId = m.SubjectId,
                SubjectName = m.Subject.Name,
                ObtainedMarks = m.ObtainedMarks,
                MaxMarks = m.Subject.MaxMarks,
                ExamType = m.ExamType,
                Remarks = m.Remarks,
                ExamDate = m.ExamDate,
                Percentage = Math.Round((double)m.ObtainedMarks / m.Subject.MaxMarks * 100, 2),
                Grade = PerformanceAnalyzer.GetGrade((double)m.ObtainedMarks / m.Subject.MaxMarks * 100)
            }).ToListAsync());
        }
    }
}