using System.Collections.Generic;

namespace backend.Dto
{
    public class CreateSalesDto
    {
        public int UserId { get; set; }
        public int StaffId { get; set; }
        public decimal Discount { get; set; }
        public string? PaymentStatus { get; set; }
        
        public List<CreateSalesItemDto> SalesItems { get; set; } = new List<CreateSalesItemDto>();
    }
}
