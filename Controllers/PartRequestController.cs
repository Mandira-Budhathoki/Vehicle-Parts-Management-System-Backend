using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/part-requests")]
    [ApiController]
    [Authorize]
    public class PartRequestController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PartRequestController(AppDbContext context)
        {
            _context = context;
        }


        // GET: api/part-requests/user/{userId}
        // Returns all part requests submitted by a customer
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var requests = await _context.PartRequests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new PartRequestDto
                {
                    RequestId = r.RequestId,
                    PartName = r.PartName,
                    Notes = r.Part != null ? r.Part.Description : null,
                    Status = r.Status,
                    RequestDate = r.RequestDate
                })
                .ToListAsync();

            return Ok(requests);
        }

        // POST: api/part-requests
        // Submit a new part request (customer)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (string.IsNullOrWhiteSpace(dto.PartName))
                return BadRequest(new { message = "Part name is required." });

            var request = new PartRequest
            {
                UserId = userId,
                PartName = dto.PartName.Trim(),
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.PartRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Part request submitted successfully.", requestId = request.RequestId });
        }

        

        // GET: api/part-requests/all
        // Returns all part requests for staff to review
        [HttpGet("all")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.PartRequests
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(r => r.Status == status);

            var requests = await query
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new StaffPartRequestDto
                {
                    RequestId = r.RequestId,
                    PartName = r.PartName,
                    Notes = r.Part != null ? r.Part.Description : null,
                    Status = r.Status,
                    RequestDate = r.RequestDate,
                    CustomerName = r.User.Name,
                    CustomerEmail = r.User.Email,
                    CustomerId = r.UserId
                })
                .ToListAsync();

            return Ok(requests);
        }

        // PUT: api/part-requests/{id}/status
        // Staff marks a request as Fulfilled or Rejected, notifies the customer
        [HttpPut("{id}/status")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePartRequestStatusDto dto)
        {
            var allowed = new[] { "Fulfilled", "Rejected", "Pending" };
            if (!allowed.Contains(dto.Status))
                return BadRequest(new { message = $"Status must be one of: {string.Join(", ", allowed)}" });

            var request = await _context.PartRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound(new { message = "Part request not found." });

            request.Status = dto.Status;
            await _context.SaveChangesAsync();

            // ── Notify the customer ───────────────────────────────────────────
            var message = dto.Status switch
            {
                "Fulfilled" => $"Great news! Your part request for \"{request.PartName}\" has been FULFILLED and is now available in our inventory.",
                "Rejected" => $"Your part request for \"{request.PartName}\" could not be fulfilled at this time. Please contact us for more details.",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(message))
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = request.UserId,
                    Message = message,
                    Type = "PartRequest",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Part request marked as {dto.Status}." });
        }
    }
}
