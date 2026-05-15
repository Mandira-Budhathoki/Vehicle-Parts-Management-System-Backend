using System.ComponentModel.DataAnnotations;

namespace backend.Dto
{
    public class CreatePartDto
    {
        [Required]
        [MaxLength(100)]
        public string PartName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Range(0, int.MaxValue)]
        public int ReorderLevel { get; set; }

        public int? VendorId { get; set; }
    }
}
