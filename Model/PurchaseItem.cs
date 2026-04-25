namespace backend.Model
{
    public class PurchaseItem
    {
        public int PurchaseItemId { get; set; }
        public int PurchaseId { get; set; }
        public int PartId { get; set; }

        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Subtotal { get; set; }

        public Purchase Purchase { get; set; }
        public Part Part { get; set; }
    }
}