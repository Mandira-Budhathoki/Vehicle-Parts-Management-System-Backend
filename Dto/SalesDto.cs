using System;
using System.Collections.Generic;

namespace backend.Dto
{
    public class SalesDto
    {
        public int SalesId { get; set; }
        public int UserId { get; set; }
        public string CustomerName { get; set; }
        public int StaffId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentStatus { get; set; }
        
        public List<SalesItemDto> SalesItems { get; set; } = new List<SalesItemDto>();
    }
}
