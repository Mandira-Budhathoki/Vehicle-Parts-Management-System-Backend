using backend.Data;
using backend.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public NotificationController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
            {
                return userId;
            }
            throw new Exception("User ID not found in token.");
        }

        // GET: api/notification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            try
            {
                int userId = GetUserId();
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(50) // Limit to latest 50 notifications
                    .Select(n => new NotificationDto
                    {
                        NotificationId = n.NotificationId,
                        Message = n.Message,
                        Type = n.Type,
                        IsRead = n.IsRead,
                        CreatedAt = n.CreatedAt
                    })
                    .ToListAsync();

                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving notifications", detail = ex.Message });
            }
        }

        // PUT: api/notification/{id}/read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                int userId = GetUserId();
                var notification = await _context.Notifications
                    .FirstOrDefaultAsync(n => n.NotificationId == id && n.UserId == userId);

                if (notification == null)
                    return NotFound(new { message = "Notification not found or unauthorized." });

                notification.IsRead = true;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Notification marked as read." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating notification", detail = ex.Message });
            }
        }

        // PUT: api/notification/read-all
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                int userId = GetUserId();
                var unreadNotifications = await _context.Notifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                if (!unreadNotifications.Any())
                    return Ok(new { message = "No unread notifications." });

                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "All notifications marked as read." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating notifications", detail = ex.Message });
            }
        }
    }
}
