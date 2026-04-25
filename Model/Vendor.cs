namespace backend.Model
{
    public class Vendor
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public List<Part> Parts { get; set; }
        public List<Purchase> Purchases { get; set; }
    }
}