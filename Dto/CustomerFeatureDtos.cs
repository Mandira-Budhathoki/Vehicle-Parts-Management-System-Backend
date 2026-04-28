namespace backend.Dto
{
    // Book Appointment
    public class CreateAppointmentDto
    {
        public int VehicleId { get; set; }
        public DateTime Date { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    // Submit Review
    public class CreateReviewDto
    {
        public int VehicleId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Request Unavailable Part
    public class CreatePartRequestDto
    {
        public string PartName { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class PartRequestDto
    {
        public int RequestId { get; set; }
        public string PartName { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
    }

    // ── Feature 14: Sales History ─────────────────────────────────────────────
    public class SalesHistoryDto
    {
        public int SalesId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<SalesItemHistoryDto> Items { get; set; } = new();
    }

    public class SalesItemHistoryDto
    {
        public string PartName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
    }

    // Email Invoice 
    public class SendInvoiceEmailDto
    {
        public int SalesId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
    }
}