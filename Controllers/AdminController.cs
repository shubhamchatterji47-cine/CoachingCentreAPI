using CoachingAPI.Data;
using CoachingAPI.DTOs.ClassSubjectDTOs;
using CoachingAPI.DTOs.CourseDTOs;
using CoachingAPI.DTOs.DashboardDTOs;
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
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db) { _db = db; }

        // ─── DASHBOARD ───────────────────────────────────────────────────────────

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            return Ok(new AdminDashboardDto
            {
                TotalStudents = await _db.Students.CountAsync(),
                TotalTeachers = await _db.Teachers.CountAsync(),
                TotalCourses = await _db.Courses.CountAsync(c => c.IsActive),
                TotalClasses = await _db.Classes.CountAsync(c => c.IsActive),
                Categories = await _db.Categories.Where(c => c.IsActive)
                    .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description, Icon = c.Icon })
                    .ToListAsync(),
                RecentAnnouncements = await _db.Announcements.Where(a => a.IsActive)
                    .OrderByDescending(a => a.CreatedAt).Take(5)
                    .Select(a => new AnnouncementDto { Id = a.Id, Title = a.Title, Content = a.Content, TargetRole = a.TargetRole, CreatedAt = a.CreatedAt })
                    .ToListAsync()
            });
        }

        // ─── TEACHERS ────────────────────────────────────────────────────────────

        [HttpGet("teachers")]
        public async Task<IActionResult> GetTeachers()
        {
            return Ok(await _db.Teachers
                .Include(t => t.User)
                .Select(t => new TeacherDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = t.User.FullName,
                    Email = t.User.Email,
                    PhoneNumber = t.User.PhoneNumber,
                    Qualification = t.Qualification,
                    Specialization = t.Specialization,
                    ExperienceYears = t.ExperienceYears,
                    Bio = t.Bio,
                    IsActive = t.User.IsActive
                }).ToListAsync());
        }

        [HttpPost("teachers")]
        public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(new { message = "A user with this email already exists." });

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Teacher",
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var teacher = new Teacher
            {
                UserId = user.Id,
                Qualification = dto.Qualification,
                Specialization = dto.Specialization,
                ExperienceYears = dto.ExperienceYears,
                Bio = dto.Bio
            };
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Teacher created successfully.", teacherId = teacher.Id });
        }

        // ─── FIX 2: UpdateTeacher was MISSING — added here ───────────────────────
        [HttpPut("teachers/{id}")]
        public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherDto dto)
        {
            var teacher = await _db.Teachers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound(new { message = "Teacher not found." });

            teacher.User.FullName = dto.FullName;
            teacher.User.PhoneNumber = dto.PhoneNumber;
            teacher.Qualification = dto.Qualification;
            teacher.Specialization = dto.Specialization;
            teacher.ExperienceYears = dto.ExperienceYears;
            teacher.Bio = dto.Bio;

            await _db.SaveChangesAsync();
            return Ok(new { message = "Teacher updated successfully." });
        }

        [HttpDelete("teachers/{id}")]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var teacher = await _db.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound(new { message = "Teacher not found." });
            teacher.User.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Teacher deactivated." });
        }

        [HttpPut("teachers/{id}/toggle")]
        public async Task<IActionResult> ToggleTeacher(int id)
        {
            var teacher = await _db.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound(new { message = "Teacher not found." });
            teacher.User.IsActive = !teacher.User.IsActive;
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Teacher {(teacher.User.IsActive ? "activated" : "deactivated")}.", isActive = teacher.User.IsActive });
        }

        [HttpDelete("teachers/{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteTeacher(int id)
        {
            var teacher = await _db.Teachers.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
            if (teacher == null) return NotFound(new { message = "Teacher not found." });
            _db.Teachers.Remove(teacher);
            _db.Users.Remove(teacher.User);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Teacher permanently deleted." });
        }

        // ─── STUDENTS ────────────────────────────────────────────────────────────

        [HttpPut("students/{id}/toggle")]
        public async Task<IActionResult> ToggleStudent(int id)
        {
            var student = await _db.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound(new { message = "Student not found." });
            student.User.IsActive = !student.User.IsActive;
            await _db.SaveChangesAsync();
            return Ok(new { message = $"Student {(student.User.IsActive ? "activated" : "deactivated")}.", isActive = student.User.IsActive });
        }

        [HttpDelete("students/{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteStudent(int id)
        {
            var student = await _db.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound(new { message = "Student not found." });
            _db.Students.Remove(student);
            _db.Users.Remove(student.User);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Student permanently deleted." });
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents([FromQuery] int? classId)
        {
            var query = _db.Students
                .Include(s => s.User)
                .Include(s => s.Class).ThenInclude(c => c.Course)
                .AsQueryable();

            if (classId.HasValue) query = query.Where(s => s.ClassId == classId);

            return Ok(await query.Select(s => new StudentDto
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
                // ─── FIX 1: ParentPhone & Address were missing from SELECT ───
                ParentPhone = s.ParentPhone,
                Address = s.Address,
                EnrollmentDate = s.EnrollmentDate,
                IsActive = s.User.IsActive
            }).ToListAsync());
        }

        [HttpPost("students/add")]
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

            return Ok(new { message = $"Student '{dto.FullName}' added successfully.", studentId = student.Id });
        }

        // ─── CATEGORIES ──────────────────────────────────────────────────────────

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _db.Categories.Where(c => c.IsActive)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Description = c.Description, Icon = c.Icon })
                .ToListAsync());
        }

        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            var cat = new Category { Name = dto.Name, Description = dto.Description, Icon = dto.Icon };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Category created.", categoryId = cat.Id });
        }

        // ─── COURSES ─────────────────────────────────────────────────────────────

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            return Ok(await _db.Courses.Include(c => c.Category).Include(c => c.Classes)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.Name,
                    Fee = c.Fee,
                    DurationMonths = c.DurationMonths,
                    IsActive = c.IsActive,
                    ClassCount = c.Classes.Count
                }).ToListAsync());
        }

        [HttpPost("courses")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto dto)
        {
            var course = new Course
            {
                Name = dto.Name,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Fee = dto.Fee,
                DurationMonths = dto.DurationMonths,
                IsActive = true
            };
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Course created.", courseId = course.Id });
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _db.Courses.Include(c => c.Classes).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) return NotFound(new { message = "Course not found." });
            if (course.Classes.Any(c => c.IsActive))
                return BadRequest(new { message = "Cannot delete course with active classes. Delete classes first." });
            course.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Course deactivated successfully." });
        }

        [HttpPut("courses/{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CreateCourseDto dto)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound(new { message = "Course not found." });
            course.Name = dto.Name;
            course.Description = dto.Description;
            course.CategoryId = dto.CategoryId;
            course.Fee = dto.Fee;
            course.DurationMonths = dto.DurationMonths;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Course updated successfully." });
        }

        // ─── SUBJECTS ────────────────────────────────────────────────────────────

        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects([FromQuery] int? courseId)
        {
            var query = _db.Subjects.Include(s => s.Course).AsQueryable();
            if (courseId.HasValue) query = query.Where(s => s.CourseId == courseId);
            return Ok(await query.Select(s => new SubjectDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CourseId = s.CourseId,
                CourseName = s.Course.Name,
                MaxMarks = s.MaxMarks,
                PassingMarks = s.PassingMarks
            }).ToListAsync());
        }

        [HttpPost("subjects")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            var subject = new Subject
            {
                Name = dto.Name,
                Description = dto.Description,
                CourseId = dto.CourseId,
                MaxMarks = dto.MaxMarks,
                PassingMarks = dto.PassingMarks
            };
            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Subject created.", subjectId = subject.Id });
        }

        [HttpDelete("subjects/{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _db.Subjects.Include(s => s.Marks).FirstOrDefaultAsync(s => s.Id == id);
            if (subject == null) return NotFound(new { message = "Subject not found." });
            if (subject.Marks.Any())
                return BadRequest(new { message = "Cannot delete subject with existing marks." });
            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Subject deleted successfully." });
        }

        [HttpPut("subjects/{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] CreateSubjectDto dto)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null) return NotFound(new { message = "Subject not found." });
            subject.Name = dto.Name;
            subject.Description = dto.Description;
            subject.CourseId = dto.CourseId;
            subject.MaxMarks = dto.MaxMarks;
            subject.PassingMarks = dto.PassingMarks;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Subject updated successfully." });
        }

        // ─── CLASSES ─────────────────────────────────────────────────────────────

        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            return Ok(await _db.Classes.Where(c => c.IsActive)
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
                }).ToListAsync());
        }

        [HttpPost("classes")]
        public async Task<IActionResult> CreateClass([FromBody] CreateClassDto dto)
        {
            var cls = new Class
            {
                Name = dto.Name,
                Description = dto.Description,
                CourseId = dto.CourseId,
                Schedule = dto.Schedule,
                Timing = dto.Timing,
                MaxCapacity = dto.MaxCapacity,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = true
            };
            _db.Classes.Add(cls);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Class created.", classId = cls.Id });
        }

        [HttpDelete("classes/{id}")]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var cls = await _db.Classes.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == id);
            if (cls == null) return NotFound(new { message = "Class not found." });
            if (cls.Students.Any())
                return BadRequest(new { message = "Cannot delete class with enrolled students." });
            cls.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Class deactivated successfully." });
        }

        [HttpPut("classes/{id}")]
        public async Task<IActionResult> UpdateClass(int id, [FromBody] CreateClassDto dto)
        {
            var cls = await _db.Classes.FindAsync(id);
            if (cls == null) return NotFound(new { message = "Class not found." });
            cls.Name = dto.Name;
            cls.Description = dto.Description;
            cls.CourseId = dto.CourseId;
            cls.Schedule = dto.Schedule;
            cls.Timing = dto.Timing;
            cls.MaxCapacity = dto.MaxCapacity;
            cls.StartDate = dto.StartDate;
            cls.EndDate = dto.EndDate;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Class updated successfully." });
        }

        // ─── ASSIGN TEACHER ──────────────────────────────────────────────────────

        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignments()
        {
            return Ok(await _db.ClassSubjects
                .Include(cs => cs.Class).ThenInclude(c => c.Course)
                .Include(cs => cs.Subject)
                .Include(cs => cs.Teacher).ThenInclude(t => t.User)
                .Select(cs => new
                {
                    id = cs.Id,
                    teacherId = cs.TeacherId,
                    teacherName = cs.Teacher.User.FullName,
                    classId = cs.ClassId,
                    className = cs.Class.Name,
                    courseName = cs.Class.Course.Name,
                    subjectId = cs.SubjectId,
                    subjectName = cs.Subject.Name
                })
                .OrderBy(cs => cs.className).ThenBy(cs => cs.subjectName)
                .ToListAsync());
        }

        [HttpDelete("assignments/{id}")]
        public async Task<IActionResult> RemoveAssignment(int id)
        {
            var cs = await _db.ClassSubjects.FindAsync(id);
            if (cs == null) return NotFound(new { message = "Assignment not found." });
            _db.ClassSubjects.Remove(cs);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Assignment removed." });
        }

        [HttpPost("assign-teacher")]
        public async Task<IActionResult> AssignTeacher([FromBody] AssignTeacherDto dto)
        {
            var exists = await _db.ClassSubjects.AnyAsync(cs =>
                cs.ClassId == dto.ClassId && cs.SubjectId == dto.SubjectId && cs.TeacherId == dto.TeacherId);

            if (exists)
                return BadRequest(new { message = "This teacher is already assigned to this subject in this class." });

            var existing = await _db.ClassSubjects
                .FirstOrDefaultAsync(cs => cs.ClassId == dto.ClassId && cs.SubjectId == dto.SubjectId);
            if (existing != null) _db.ClassSubjects.Remove(existing);

            _db.ClassSubjects.Add(new ClassSubject
            {
                ClassId = dto.ClassId,
                SubjectId = dto.SubjectId,
                TeacherId = dto.TeacherId
            });
            await _db.SaveChangesAsync();
            return Ok(new { message = "Teacher assigned successfully." });
        }

        // ─── ANNOUNCEMENTS ───────────────────────────────────────────────────────

        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            return Ok(await _db.Announcements.Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    TargetRole = a.TargetRole,
                    CreatedAt = a.CreatedAt
                }).ToListAsync());
        }

        [HttpPost("announcements")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto dto)
        {
            var ann = new Announcement { Title = dto.Title, Content = dto.Content, TargetRole = dto.TargetRole };
            _db.Announcements.Add(ann);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Announcement posted.", announcementId = ann.Id });
        }

        [HttpDelete("announcements/{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var ann = await _db.Announcements.FindAsync(id);
            if (ann == null) return NotFound();
            ann.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Announcement removed." });
        }
    }
}