namespace backend.Model
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public int VendorId { get; set; }

        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }

        public User User { get; set; }
        public Vendor Vendor { get; set; }

        public List<PurchaseItem> PurchaseItems { get; set; }
    }
}
