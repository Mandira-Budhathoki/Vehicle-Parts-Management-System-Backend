using System;
using System.Collections.Generic;

namespace backend.Dto
{
    public class FinancialReportDto
    {
        public string Period { get; set; } // "daily", "monthly", "yearly"
        public DateTime ReferenceDate { get; set; }
        
        // Summary Metrics
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalSalesCount { get; set; }
        public int TotalPurchasesCount { get; set; }
        
        // Dynamic breakdown items (by hour/interval for daily, by day of month for monthly, by month for yearly)
        public List<FinancialBreakdownItemDto> Breakdown { get; set; }
        
        // Detailed breakdowns
        public List<TopPartDto> TopSellingParts { get; set; }
        public List<RecentTransactionDto> RecentTransactions { get; set; }
    }

    public class FinancialBreakdownItemDto
    {
        public string Label { get; set; } // e.g. "10:00 AM", "May 15", "January"
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal Profit { get; set; }
    }

    public class TopPartDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string Category { get; set; }
        public int QuantitySold { get; set; }
        public decimal RevenueGenerated { get; set; }
    }

    public class RecentTransactionDto
    {
        public string Id { get; set; } // e.g. "SALE-101", "PURC-205"
        public string Type { get; set; } // "Sale", "Purchase"
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
}
