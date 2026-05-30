using CoachingAPI.Data;
using CoachingAPI.DTOs.ClassSubjectDTOs;
using CoachingAPI.DTOs.CourseDTOs;
using CoachingAPI.DTOs.DashboardDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachingAPI.Controllers
{
    /// <summary>
    /// Public endpoints — no authentication required.
    /// Used by the MVC Home, Courses, and Classes pages visible to everyone.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly AppDbContext _db;
        public PublicController(AppDbContext db) { _db = db; }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            return Ok(await _db.Courses
                .Where(c => c.IsActive)
                .Include(c => c.Category)
                .Include(c => c.Classes)
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

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _db.Categories
                .Where(c => c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon
                }).ToListAsync());
        }

        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses()
        {
            return Ok(await _db.Classes
                .Where(c => c.IsActive)
                .Include(c => c.Course)
                .Include(c => c.Students)
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

        [HttpGet("announcements")]
        public async Task<IActionResult> GetAnnouncements()
        {
            return Ok(await _db.Announcements
                .Where(a => a.IsActive && (a.TargetRole == null || a.TargetRole == ""))
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new AnnouncementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    TargetRole = a.TargetRole,
                    CreatedAt = a.CreatedAt
                }).ToListAsync());
        }
    }
}