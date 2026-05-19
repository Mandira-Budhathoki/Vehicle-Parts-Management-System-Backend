using backend.Data;
using backend.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/sales-history")]
    [ApiController]
    [Authorize]
    public class SalesHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesHistoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/sales-history/user/{userId}

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(
            int userId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? search)
        {
            var query = _context.Sales
                .Include(s => s.SalesItems)
                    .ThenInclude(si => si.Part)
                .Where(s => s.UserId == userId)
                .AsQueryable();

            // Date range filters
            if (from.HasValue)
                query = query.Where(s => s.Date >= from.Value.ToUniversalTime());

            if (to.HasValue)
                // include the full "to" day
                query = query.Where(s => s.Date <= to.Value.ToUniversalTime().AddDays(1).AddTicks(-1));

            // Part name search: keep the sale if ANY item matches
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                query = query.Where(s =>
                    s.SalesItems.Any(si =>
                        si.Part != null &&
                        si.Part.PartName.ToLower().Contains(lower)));
            }

            var sales = await query
                .OrderByDescending(s => s.Date)
                .Select(s => new SalesHistoryDto
                {
                    SalesId = s.SalesId,
                    Date = s.Date,
                    TotalAmount = s.TotalAmount,
                    Discount = s.Discount,
                    FinalAmount = s.FinalAmount,
                    PaymentStatus = s.PaymentStatus,
                    Items = s.SalesItems.Select(si => new SalesItemHistoryDto
                    {
                        PartName = si.Part.PartName,
                        Quantity = si.Quantity,
                        Price = si.Price,
                        Subtotal = si.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            return Ok(sales);
        }
    }
}
