using CoachingAPI.Data;
using CoachingAPI.DTOs.AuthDTOs;
using CoachingAPI.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoachingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtHelper _jwt;

        public AuthController(AppDbContext db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.IsActive);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            int? roleEntityId = null;
            if (user.Role == "Teacher")
            {
                var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                roleEntityId = teacher?.Id;
            }
            else if (user.Role == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                roleEntityId = student?.Id;
            }

            var token = _jwt.GenerateToken(user, roleEntityId);
            return Ok(new LoginResponseDto
            {
                Token = token,
                Role = user.Role,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleEntityId = roleEntityId
            });
        }

        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect." });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Password changed successfully." });
        }
    }
}