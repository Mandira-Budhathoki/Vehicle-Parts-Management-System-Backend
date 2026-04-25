namespace backend.Model
{
    public class SalesItem
    {
        public int SalesItemId { get; set; }
        public int SalesId { get; set; }
        public int PartId { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }

        public Sales Sales { get; set; }
        public Part Part { get; set; }
    }
}