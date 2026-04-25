namespace backend.Model
{
    public class Sales
    {
        public int SalesId { get; set; }
        public int UserId { get; set; }
        public int StaffId { get; set; }

        public DateTime Date { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }

        public string PaymentStatus { get; set; }

        public User User { get; set; }
        public List<SalesItem> SalesItems { get; set; }
    }
}