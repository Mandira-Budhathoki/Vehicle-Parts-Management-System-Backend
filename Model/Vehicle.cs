namespace backend.Model
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int UserId { get; set; }

        public string VehicleNumber { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        public int Year { get; set; }

        public User User { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Review> Reviews { get; set; }
    }
}