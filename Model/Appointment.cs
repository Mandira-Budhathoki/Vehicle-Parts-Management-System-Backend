using System.ComponentModel.DataAnnotations;

namespace backend.Model
{
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public int VehicleId { get; set; }

        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public string Notes { get; set; }

        public Vehicle Vehicle { get; set; }
    }
}