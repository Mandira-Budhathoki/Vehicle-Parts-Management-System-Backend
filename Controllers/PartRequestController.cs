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
        // All part requests from a customer
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
        // Submit a new part request 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartRequestDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (string.IsNullOrWhiteSpace(dto.PartName))
                return BadRequest(new { message = "Part name is required." });

            var request = new PartRequest
            {
                UserId = userId,
                PartName = dto.PartName,
                Status = "Pending",
                RequestDate = DateTime.UtcNow
            };

            _context.PartRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Part request submitted successfully.", requestId = request.RequestId });
        }
    }
}