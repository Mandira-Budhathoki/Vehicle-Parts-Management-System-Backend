namespace backend.Dto
{
    public class RegularCustomerDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }

    public class HighSpenderDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int TotalOrders { get; set; }
        public bool HasLoyaltyDiscount { get; set; }
    }

    public class PendingCreditDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int SalesId { get; set; }
        public decimal AmountDue { get; set; }
        public DateTime SaleDate { get; set; }
        public int DaysOverdue { get; set; }
    }
}