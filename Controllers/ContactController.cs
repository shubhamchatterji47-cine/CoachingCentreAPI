using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CoachingAPI.Data;
using CoachingAPI.Models;

namespace CoachingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ContactController(AppDbContext db) { _db = db; }

        // PUBLIC — anyone can send a message
        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] ContactMessage dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest(new { message = "Name, email and message are required." });

            var msg = new ContactMessage
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Subject = dto.Subject,
                Message = dto.Message,
                SentAt = DateTime.UtcNow
            };
            _db.ContactMessages.Add(msg);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Message sent successfully! We'll get back to you soon." });
        }

        // ADMIN — get all messages
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _db.ContactMessages.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(m => m.Status == status);

            return Ok(await query.OrderByDescending(m => m.SentAt).ToListAsync());
        }

        // ADMIN — mark as read
        [HttpPut("{id}/read")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var msg = await _db.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();
            msg.IsRead = true;
            msg.Status = "Read";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Marked as read." });
        }

        // ADMIN — reply
        [HttpPut("{id}/reply")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reply(int id, [FromBody] dynamic dto)
        {
            var msg = await _db.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();
            msg.AdminReply = dto.reply?.ToString() ?? "";
            msg.Status = "Replied";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Reply saved." });
        }

        // ADMIN — delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var msg = await _db.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();
            _db.ContactMessages.Remove(msg);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Message deleted." });
        }

        // ADMIN — unread count
        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Count()
        {
            return Ok(new
            {
                total = await _db.ContactMessages.CountAsync(),
                unread = await _db.ContactMessages.CountAsync(m => !m.IsRead),
                replied = await _db.ContactMessages.CountAsync(m => m.Status == "Replied")
            });
        }
    }
}