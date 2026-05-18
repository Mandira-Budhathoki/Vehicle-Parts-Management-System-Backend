using backend.Data;
using backend.Dto;
using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;

        public ReportsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RegularCustomerDto>> GetRegularCustomersAsync()
        {
            return await _context.Sales
                .Where(s => s.User != null && s.User.Role.ToUpper() == "CUSTOMER")
                .GroupBy(s => new { s.UserId, s.User.Name, s.User.Email, s.User.Phone })
                .Where(g => g.Count() >= 3)
                .Select(g => new RegularCustomerDto
                {
                    UserId = g.Key.UserId,
                    Name = g.Key.Name,
                    Email = g.Key.Email,
                    Phone = g.Key.Phone,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(s => s.FinalAmount),
                    LastPurchaseDate = g.Max(s => s.Date)
                })
                .OrderByDescending(c => c.TotalOrders)
                .ToListAsync();
        }

        public async Task<IEnumerable<HighSpenderDto>> GetHighSpendersAsync()
        {
            return await _context.Sales
                .Where(s => s.User != null && s.User.Role.ToUpper() == "CUSTOMER")
                .GroupBy(s => new { s.UserId, s.User.Name, s.User.Email, s.User.Phone })
                .Select(g => new HighSpenderDto
                {
                    UserId = g.Key.UserId,
                    Name = g.Key.Name,
                    Email = g.Key.Email,
                    Phone = g.Key.Phone,
                    TotalSpent = g.Sum(s => s.FinalAmount),
                    TotalOrders = g.Count(),
                    HasLoyaltyDiscount = g.Sum(s => s.TotalAmount) >= 5000
                })
                .Where(c => c.TotalSpent > 2000)
                .OrderByDescending(c => c.TotalSpent)
                .ToListAsync();
        }

        public async Task<IEnumerable<PendingCreditDto>> GetPendingCreditsAsync()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-30);

            return await _context.Sales
                .Include(s => s.User)
                .Where(s =>
                    s.PaymentStatus == "PENDING" &&
                    s.Date < cutoffDate &&
                    s.User != null &&
                    s.User.Role.ToUpper() == "CUSTOMER")
                .Select(s => new PendingCreditDto
                {
                    UserId = s.UserId,
                    Name = s.User.Name,
                    Email = s.User.Email,
                    Phone = s.User.Phone,
                    SalesId = s.SalesId,
                    AmountDue = s.FinalAmount,
                    SaleDate = s.Date,
                    DaysOverdue = (int)(DateTime.UtcNow - s.Date).TotalDays
                })
                .OrderByDescending(c => c.DaysOverdue)
                .ToListAsync();
        }
    }
}