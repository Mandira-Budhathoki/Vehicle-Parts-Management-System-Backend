using System.Collections.Generic;

namespace backend.Dto
{
    public class CustomerResponseDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal TotalSpent { get; set; }
        public List<VehicleDto> Vehicles { get; set; } = new List<VehicleDto>();
    }
}
