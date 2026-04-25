namespace backend.Model
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int SalesId { get; set; }

        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DueDate { get; set; }

        public string Status { get; set; }

        public Sales Sales { get; set; }
    }
}
