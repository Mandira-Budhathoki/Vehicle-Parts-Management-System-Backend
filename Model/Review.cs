namespace backend.Model
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int VehicleId { get; set; }
        public int UserId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Vehicle Vehicle { get; set; }
        public User User { get; set; }
    }
}