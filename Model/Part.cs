using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace backend.Model
{
    public class Part
    {
        [Key]
        public int PartId { get; set; }

        public string PartName { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }

        public int? VendorId { get; set; }
        public Vendor Vendor { get; set; }

        public List<SalesItem> SalesItems { get; set; }
        public List<PurchaseItem> PurchaseItems { get; set; }
        public string StockStatus { get; set; } = "NORMAL";
    }
}
