namespace backend.Services
{
    using backend.Data;
    using backend.Dto;
    using backend.Interfaces;
    using backend.Model;

    public class VehicleService : IVehicleService
    {
        private readonly AppDbContext _context;

        public VehicleService(AppDbContext context)
        {
            _context = context;
        }

        public void AddVehicle(int userId, CreateVehicleDto dto)
        {
            var vehicle = new Vehicle
            {
                UserId = userId,
                VehicleNumber = dto.VehicleNumber,
                Model = dto.Model,
                Brand = dto.Brand,
                Year = dto.Year
            };

            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public List<VehicleDto> GetVehicles(int userId)
        {
            return _context.Vehicles
                .Where(v => v.UserId == userId)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    Model = v.Model,
                    Brand = v.Brand,
                    Year = v.Year
                }).ToList();
        }

        public void UpdateVehicle(int id, CreateVehicleDto dto)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null) return;

            vehicle.VehicleNumber = dto.VehicleNumber;
            vehicle.Model = dto.Model;
            vehicle.Brand = dto.Brand;
            vehicle.Year = dto.Year;

            _context.SaveChanges();
        }

        public void DeleteVehicle(int id)
        {
            var vehicle = _context.Vehicles.Find(id);
            if (vehicle == null) return;

            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }
    }
}
