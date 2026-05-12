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
        // Returns all purchase/sales records for a customer 
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var sales = await _context.Sales
                .Include(s => s.SalesItems)
                    .ThenInclude(si => si.Part)
                .Where(s => s.UserId == userId)
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