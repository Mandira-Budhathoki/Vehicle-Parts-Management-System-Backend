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
                    HasLoyaltyDiscount = g.Sum(s => s.FinalAmount) >= 5000
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

        public async Task<FinancialReportDto> GetFinancialReportAsync(string period, DateTime referenceDate)
        {
            period = period.ToLower();
            DateTime startDate;
            DateTime endDate;

            if (period == "daily")
            {
                startDate = DateTime.SpecifyKind(referenceDate.Date, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(startDate.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            }
            else if (period == "yearly")
            {
                startDate = new DateTime(referenceDate.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(startDate.AddYears(1).AddTicks(-1), DateTimeKind.Utc);
            }
            else // Default to monthly
            {
                startDate = new DateTime(referenceDate.Year, referenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                endDate = DateTime.SpecifyKind(startDate.AddMonths(1).AddTicks(-1), DateTimeKind.Utc);
                period = "monthly"; // Normalize
            }

            // 1. Fetch Sales and Purchases in the period
            var sales = await _context.Sales
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .ToListAsync();

            var purchases = await _context.Purchases
                .Where(p => p.Date >= startDate && p.Date <= endDate)
                .ToListAsync();

            // 2. Metrics Summary
            var totalRevenue = sales.Sum(s => s.FinalAmount);
            var totalExpenses = purchases.Sum(p => p.TotalAmount);
            var netProfit = totalRevenue - totalExpenses;
            var totalSalesCount = sales.Count;
            var totalPurchasesCount = purchases.Count;

            // 3. Breakdown calculations based on period
            var breakdown = new List<FinancialBreakdownItemDto>();

            if (period == "daily")
            {
                // Hourly breakdown (00:00 to 23:00, grouping by hour)
                for (int hour = 0; hour < 24; hour++)
                {
                    var hourSales = sales.Where(s => s.Date.Hour == hour).Sum(s => s.FinalAmount);
                    var hourPurchases = purchases.Where(p => p.Date.Hour == hour).Sum(p => p.TotalAmount);
                    
                    // Only add if there are sales or purchases or if it's typical working hours (9 AM to 6 PM)
                    if (hourSales > 0 || hourPurchases > 0 || (hour >= 9 && hour <= 18))
                    {
                        var suffix = hour >= 12 ? "PM" : "AM";
                        var displayHour = hour % 12 == 0 ? 12 : hour % 12;
                        var label = $"{displayHour:D2}:00 {suffix}";
                        
                        breakdown.Add(new FinancialBreakdownItemDto
                        {
                            Label = label,
                            Revenue = hourSales,
                            Expenses = hourPurchases,
                            Profit = hourSales - hourPurchases
                        });
                    }
                }
            }
            else if (period == "yearly")
            {
                // Monthly breakdown (January to December)
                string[] monthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
                for (int month = 1; month <= 12; month++)
                {
                    var monthSales = sales.Where(s => s.Date.Month == month).Sum(s => s.FinalAmount);
                    var monthPurchases = purchases.Where(p => p.Date.Month == month).Sum(p => p.TotalAmount);

                    breakdown.Add(new FinancialBreakdownItemDto
                    {
                        Label = monthNames[month - 1],
                        Revenue = monthSales,
                        Expenses = monthPurchases,
                        Profit = monthSales - monthPurchases
                    });
                }
            }
            else // Monthly
            {
                // Daily breakdown for the selected month
                int daysInMonth = DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var daySales = sales.Where(s => s.Date.Day == day).Sum(s => s.FinalAmount);
                    var dayPurchases = purchases.Where(p => p.Date.Day == day).Sum(p => p.TotalAmount);

                    breakdown.Add(new FinancialBreakdownItemDto
                    {
                        Label = $"{day}",
                        Revenue = daySales,
                        Expenses = dayPurchases,
                        Profit = daySales - dayPurchases
                    });
                }
            }

            // 4. Fetch Top Selling Parts during the period
            var topSellingParts = await _context.Sales
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .SelectMany(s => s.SalesItems)
                .GroupBy(si => new { si.PartId, si.Part.PartName, si.Part.Category })
                .Select(g => new TopPartDto
                {
                    PartId = g.Key.PartId,
                    PartName = g.Key.PartName,
                    Category = g.Key.Category,
                    QuantitySold = g.Sum(si => si.Quantity),
                    RevenueGenerated = g.Sum(si => si.Subtotal)
                })
                .OrderByDescending(tp => tp.QuantitySold)
                .Take(5)
                .ToListAsync();

            // 5. Fetch Recent Transactions in the period (combine sales and purchases, sort by Date DESC, take 10)
            var recentSales = sales
                .Select(s => new RecentTransactionDto
                {
                    Id = $"SALE-{s.SalesId}",
                    Type = "Sale",
                    Date = s.Date,
                    Description = $"Invoice for Customer ID {s.UserId}",
                    Amount = s.FinalAmount,
                    Status = s.PaymentStatus
                });

            var recentPurchases = purchases
                .Select(p => new RecentTransactionDto
                {
                    Id = $"PURC-{p.PurchaseId}",
                    Type = "Purchase",
                    Date = p.Date,
                    Description = $"Purchase Invoice from Vendor ID {p.VendorId}",
                    Amount = p.TotalAmount,
                    Status = "COMPLETED"
                });

            var recentTransactions = recentSales.Concat(recentPurchases)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            return new FinancialReportDto
            {
                Period = period,
                ReferenceDate = referenceDate,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                TotalSalesCount = totalSalesCount,
                TotalPurchasesCount = totalPurchasesCount,
                Breakdown = breakdown,
                TopSellingParts = topSellingParts,
                RecentTransactions = recentTransactions
            };
        }
    }
}