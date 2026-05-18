namespace backend.Dto
{
    public class SalesItemDto
    {
        public int SalesItemId { get; set; }
        public int SalesId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }
}
