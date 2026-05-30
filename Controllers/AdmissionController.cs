using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CoachingAPI.Data;
using CoachingAPI.DTOs.AdmissionDTOs;
using CoachingAPI.Models;

namespace CoachingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdmissionController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdmissionController(AppDbContext db) { _db = db; }

        // PUBLIC — anyone can submit an application
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] CreateAdmissionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Prevent duplicate applications from same email
            var existing = await _db.AdmissionApplications
                .FirstOrDefaultAsync(a => a.Email.ToLower() == dto.Email.ToLower()
                    && a.Status == "Pending");
            if (existing != null)
                return BadRequest(new { message = "An application with this email is already pending. We will contact you soon." });

            var app = new AdmissionApplication
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ParentName = dto.ParentName,
                ParentPhone = dto.ParentPhone,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                PreviousSchool = dto.PreviousSchool,
                LastClassPassed = dto.LastClassPassed,
                CourseId = dto.CourseId,
                CourseInterest = dto.CourseInterest,
                Message = dto.Message,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            _db.AdmissionApplications.Add(app);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Application submitted successfully! We will contact you within 24 hours.", applicationId = app.Id });
        }

        // ADMIN — get all applications
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.AdmissionApplications
                .Include(a => a.Course)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(a => a.Status == status);

            var list = await query.OrderByDescending(a => a.AppliedAt)
                .Select(a => new AdmissionApplicationDto
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber,
                    ParentName = a.ParentName,
                    ParentPhone = a.ParentPhone,
                    DateOfBirth = a.DateOfBirth,
                    Address = a.Address,
                    PreviousSchool = a.PreviousSchool,
                    LastClassPassed = a.LastClassPassed,
                    CourseId = a.CourseId,
                    CourseName = a.Course != null ? a.Course.Name : null,
                    CourseInterest = a.CourseInterest,
                    Message = a.Message,
                    Status = a.Status,
                    AdminNotes = a.AdminNotes,
                    AppliedAt = a.AppliedAt,
                    ReviewedAt = a.ReviewedAt
                }).ToListAsync();

            return Ok(list);
        }

        // ADMIN — get single application
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var a = await _db.AdmissionApplications
                .Include(x => x.Course)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (a == null) return NotFound(new { message = "Application not found." });

            return Ok(new AdmissionApplicationDto
            {
                Id = a.Id,
                FullName = a.FullName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                ParentName = a.ParentName,
                ParentPhone = a.ParentPhone,
                DateOfBirth = a.DateOfBirth,
                Address = a.Address,
                PreviousSchool = a.PreviousSchool,
                LastClassPassed = a.LastClassPassed,
                CourseId = a.CourseId,
                CourseName = a.Course?.Name,
                CourseInterest = a.CourseInterest,
                Message = a.Message,
                Status = a.Status,
                AdminNotes = a.AdminNotes,
                AppliedAt = a.AppliedAt,
                ReviewedAt = a.ReviewedAt
            });
        }

        // ADMIN — update status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAdmissionStatusDto dto)
        {
            var app = await _db.AdmissionApplications.FindAsync(id);
            if (app == null) return NotFound(new { message = "Application not found." });

            app.Status = dto.Status;
            app.AdminNotes = dto.AdminNotes;
            app.ReviewedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { message = $"Application {dto.Status.ToLower()} successfully." });
        }

        // ADMIN — delete application
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var app = await _db.AdmissionApplications.FindAsync(id);
            if (app == null) return NotFound(new { message = "Application not found." });
            _db.AdmissionApplications.Remove(app);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Application deleted." });
        }

        // PUBLIC — get count by status (for dashboard badge)
        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCounts()
        {
            return Ok(new
            {
                total = await _db.AdmissionApplications.CountAsync(),
                pending = await _db.AdmissionApplications.CountAsync(a => a.Status == "Pending"),
                approved = await _db.AdmissionApplications.CountAsync(a => a.Status == "Approved"),
                rejected = await _db.AdmissionApplications.CountAsync(a => a.Status == "Rejected"),
                reviewed = await _db.AdmissionApplications.CountAsync(a => a.Status == "Reviewed")
            });
        }
    }
}