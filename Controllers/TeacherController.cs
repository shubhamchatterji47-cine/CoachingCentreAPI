using CoachingAPI.Data;
using CoachingAPI.DTOs;
using CoachingAPI.DTOs.ClassSubjectDTOs;
using CoachingAPI.DTOs.MarksDTOs;
using CoachingAPI.DTOs.StudentDTOs;
using CoachingAPI.DTOs.TeacherDTOs;
using CoachingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher,Admin")]
    public class TeacherController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TeacherController(AppDbContext db) { _db = db; }

        private int GetTeacherId() =>
            int.Parse(User.FindFirst("RoleEntityId")?.Value ?? "0");

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var teacherId = GetTeacherId();
            var assignedClasses = await _db.ClassSubjects
                .Where(cs => cs.TeacherId == teacherId)
                .Select(cs => cs.ClassId).Distinct().ToListAsync();

            return Ok(new TeacherDashboardDto
            {
                TotalStudents = await _db.Students.CountAsync(s => assignedClasses.Contains(s.ClassId)),
                TotalClasses = assignedClasses.Count,
                AssignedClasses = await _db.Classes
                    .Where(c => assignedClasses.Contains(c.Id))
                    .Include(c => c.Course).Include(c => c.Students)
                    .Select(c => new ClassDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        CourseId = c.CourseId,
                        CourseName = c.Course.Name,
                        Schedule = c.Schedule,
                        Timing = c.Timing,
                        MaxCapacity = c.MaxCapacity,
                        EnrolledCount = c.Students.Count,
                        IsActive = c.IsActive,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    }).ToListAsync()
            });
        }

        [HttpGet("my-classes")]
        public async Task<IActionResult> GetMyClasses()
        {
            var teacherId = GetTeacherId();
            var classIds = await _db.ClassSubjects
                .Where(cs => cs.TeacherId == teacherId)
                .Select(cs => cs.ClassId).Distinct().ToListAsync();
            return Ok(await _db.Classes.Where(c => classIds.Contains(c.Id))
                .Include(c => c.Course).Select(c => new ClassDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CourseName = c.Course.Name,
                    Schedule = c.Schedule,
                    Timing = c.Timing,
                    MaxCapacity = c.MaxCapacity,
                    EnrolledCount = c.Students.Count,
                    IsActive = c.IsActive,
                    StartDate = c.StartDate
                }).ToListAsync());
        }

        [HttpGet("students/{classId}")]
        public async Task<IActionResult> GetStudentsByClass(int classId)
        {
            return Ok(await _db.Students.Where(s => s.ClassId == classId)
                .Include(s => s.User).Include(s => s.Class).ThenInclude(c => c.Course)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    FullName = s.User.FullName,
                    Email = s.User.Email,
                    PhoneNumber = s.User.PhoneNumber,
                    ClassId = s.ClassId,
                    ClassName = s.Class.Name,
                    CourseName = s.Class.Course.Name,
                    RollNumber = s.RollNumber,
                    DateOfBirth = s.DateOfBirth,
                    ParentName = s.ParentName,
                    EnrollmentDate = s.EnrollmentDate,
                    IsActive = s.User.IsActive
                }).ToListAsync());
        }

        [HttpPost("students")]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new { message = "Email already exists." });

            var cls = await _db.Classes.FindAsync(dto.ClassId);
            if (cls == null) return NotFound(new { message = "Class not found." });

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Student",
                PhoneNumber = dto.PhoneNumber
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var student = new Student
            {
                UserId = user.Id,
                ClassId = dto.ClassId,
                RollNumber = dto.RollNumber,
                DateOfBirth = dto.DateOfBirth,
                ParentName = dto.ParentName,
                ParentPhone = dto.ParentPhone,
                Address = dto.Address
            };
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Student added.", studentId = student.Id });
        }

        [HttpPut("students/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
        {
            var student = await _db.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            student.User.FullName = dto.FullName; student.User.PhoneNumber = dto.PhoneNumber;
            student.RollNumber = dto.RollNumber; student.DateOfBirth = dto.DateOfBirth;
            student.ParentName = dto.ParentName; student.ParentPhone = dto.ParentPhone;
            student.Address = dto.Address; student.ClassId = dto.ClassId;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Student updated." });
        }

        // ---- SUBJECTS BY CLASS (NEW - fixes subject dropdown in Marks form) ----
        [HttpGet("subjects/{classId}")]
        public async Task<IActionResult> GetSubjectsByClass(int classId)
        {
            var cls = await _db.Classes
                .Include(c => c.Course)
                .FirstOrDefaultAsync(c => c.Id == classId);

            if (cls == null)
                return NotFound(new { message = "Class not found." });

            var subjects = await _db.Subjects
                .Where(s => s.CourseId == cls.CourseId)
                .Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    CourseId = s.CourseId,
                    CourseName = cls.Course.Name,
                    MaxMarks = s.MaxMarks,
                    PassingMarks = s.PassingMarks
                })
                .ToListAsync();

            return Ok(subjects);
        }

        // ---- MARKS ----
        [HttpGet("marks/{classId}")]
        public async Task<IActionResult> GetMarksByClass(int classId, [FromQuery] string? examType)
        {
            var query = _db.Marks.Include(m => m.Student).ThenInclude(s => s.User)
                .Include(m => m.Subject)
                .Where(m => m.Student.ClassId == classId);

            if (!string.IsNullOrEmpty(examType)) query = query.Where(m => m.ExamType == examType);

            return Ok(await query.Select(m => new MarkDto
            {
                Id = m.Id,
                StudentId = m.StudentId,
                StudentName = m.Student.User.FullName,
                SubjectId = m.SubjectId,
                SubjectName = m.Subject.Name,
                ObtainedMarks = m.ObtainedMarks,
                MaxMarks = m.Subject.MaxMarks,
                ExamType = m.ExamType,
                Remarks = m.Remarks,
                ExamDate = m.ExamDate,
                Percentage = Math.Round((double)m.ObtainedMarks / m.Subject.MaxMarks * 100, 2),
                Grade = CoachingAPI.Services.PerformanceAnalyzer.GetGrade((double)m.ObtainedMarks / m.Subject.MaxMarks * 100)
            }).ToListAsync());
        }

        [HttpPost("marks")]
        public async Task<IActionResult> AddMark([FromBody] CreateMarkDto dto)
        {
            var teacherId = GetTeacherId();
            var subject = await _db.Subjects.FindAsync(dto.SubjectId);
            if (subject == null) return NotFound(new { message = "Subject not found." });

            if (dto.ObtainedMarks < 0 || dto.ObtainedMarks > subject.MaxMarks)
                return BadRequest(new { message = $"Marks must be between 0 and {subject.MaxMarks}." });

            _db.Marks.Add(new Mark
            {
                StudentId = dto.StudentId,
                SubjectId = dto.SubjectId,
                ObtainedMarks = dto.ObtainedMarks,
                ExamType = dto.ExamType,
                Remarks = dto.Remarks,
                ExamDate = dto.ExamDate,
                EnteredByTeacherId = teacherId > 0 ? teacherId : null
            });
            await _db.SaveChangesAsync();
            return Ok(new { message = "Mark recorded." });
        }

        [HttpPost("marks/bulk")]
        public async Task<IActionResult> BulkAddMarks([FromBody] List<CreateMarkDto> dtos)
        {
            var teacherId = GetTeacherId();
            var marks = new List<Mark>();
            foreach (var dto in dtos)
            {
                var subject = await _db.Subjects.FindAsync(dto.SubjectId);
                if (subject == null) continue;
                marks.Add(new Mark
                {
                    StudentId = dto.StudentId,
                    SubjectId = dto.SubjectId,
                    ObtainedMarks = Math.Min(dto.ObtainedMarks, subject.MaxMarks),
                    ExamType = dto.ExamType,
                    Remarks = dto.Remarks,
                    ExamDate = dto.ExamDate,
                    EnteredByTeacherId = teacherId > 0 ? teacherId : null
                });
            }
            _db.Marks.AddRange(marks);
            await _db.SaveChangesAsync();
            return Ok(new { message = $"{marks.Count} marks recorded." });
        }

        [HttpPut("marks/{id}")]
        public async Task<IActionResult> UpdateMark(int id, [FromBody] CreateMarkDto dto)
        {
            var mark = await _db.Marks.FindAsync(id);
            if (mark == null) return NotFound();
            mark.ObtainedMarks = dto.ObtainedMarks; mark.ExamType = dto.ExamType;
            mark.Remarks = dto.Remarks; mark.ExamDate = dto.ExamDate;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Mark updated." });
        }

        [HttpDelete("marks/{id}")]
        public async Task<IActionResult> DeleteMark(int id)
        {
            var mark = await _db.Marks.FindAsync(id);
            if (mark == null) return NotFound();
            _db.Marks.Remove(mark);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Mark deleted." });
        }

        // ---- ATTENDANCE ----
        [HttpPost("attendance")]
        public async Task<IActionResult> MarkAttendance([FromBody] List<Attendance> records)
        {
            _db.Attendances.AddRange(records);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Attendance recorded." });
        }
    }
}