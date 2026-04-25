namespace backend.Model
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public int VehicleId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public string Notes { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}