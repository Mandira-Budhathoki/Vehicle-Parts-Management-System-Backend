using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reviews
        // Public – anyone (including staff/admin UI) can view all reviews
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Vehicle)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PublicReviewDto
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    CustomerName = r.User.Name,
                    VehicleNumber = r.Vehicle.VehicleNumber
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // GET: api/reviews/user/{userId}
        // Reviews submitted by a specific customer
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.Vehicle)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    VehicleId = r.VehicleId,
                    VehicleNumber = r.Vehicle.VehicleNumber,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // POST: api/reviews
        // Submit a review (customer) – prevents duplicate review for same vehicle
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest(new { message = "Rating must be between 1 and 5." });

            // Validate vehicle belongs to this user
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null || vehicle.UserId != userId)
                return BadRequest(new { message = "Invalid vehicle." });

            // ── Duplicate check: one review per user per vehicle ──────────────
            var alreadyReviewed = await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.VehicleId == dto.VehicleId);

            if (alreadyReviewed)
                return BadRequest(new { message = "You have already submitted a review for this vehicle." });

            var review = new Review
            {
                UserId = userId,
                VehicleId = dto.VehicleId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review submitted successfully.", reviewId = review.ReviewId });
        }
    }
}
