using System.Collections.Generic;

namespace backend.Dto
{
    public class CustomerResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
    }

    public class CustomerFullProfileDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<VehicleDto> Vehicles { get; set; } = new();
        public List<SalesHistoryDto> SalesHistory { get; set; } = new();
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<PartRequestDto> PartRequests { get; set; } = new();
    }
}
