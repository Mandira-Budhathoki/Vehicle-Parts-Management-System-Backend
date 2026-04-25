namespace backend.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // ADMIN / STAFF / CUSTOMER
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Vehicle> Vehicles { get; set; }
        public List<Sales> Sales { get; set; }
        public List<Purchase> Purchases { get; set; }
        public List<Notification> Notifications { get; set; }
    }
}
