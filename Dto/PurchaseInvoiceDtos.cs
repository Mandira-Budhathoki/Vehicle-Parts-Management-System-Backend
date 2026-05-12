using System.ComponentModel.DataAnnotations;

namespace backend.Dto
{
    public class PurchaseVendorDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class PurchaseItemDto
    {
        public int PurchaseItemId { get; set; }
        public int PartId { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class PurchaseInvoiceDto
    {
        public int PurchaseId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemDto> Items { get; set; }
    }

    public class CreatePurchaseItemDto
    {
        [Required]
        public int PartId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0.")]
        public decimal CostPrice { get; set; }
    }

    public class CreatePurchaseInvoiceDto
    {
        [Required]
        public int VendorId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required in the invoice.")]
        public List<CreatePurchaseItemDto> Items { get; set; }
    }
}
