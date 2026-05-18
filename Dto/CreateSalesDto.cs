namespace backend.Dto
{
    public class CreateSalesDto
    {
        public int UserId { get; set; }
        public int StaffId { get; set; }
        public List<SalesItemDto> Items { get; set; }
    }

    public class SalesItemDto
    {
        public int PartId { get; set; }
        public int Quantity { get; set; }
    }
}