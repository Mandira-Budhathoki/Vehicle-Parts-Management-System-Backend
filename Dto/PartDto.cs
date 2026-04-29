namespace backend.Dto
{
    public class PartDto
    {
        public int PartId { get; set; }
        public string PartName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public int? VendorId { get; set; }
    }
}
