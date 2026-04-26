using backend.Dto;

namespace backend.Interfaces
{
    public interface IVehicleService
    {
        void AddVehicle(int userId, CreateVehicleDto dto);
        List<VehicleDto> GetVehicles(int userId);
        void UpdateVehicle(int id, CreateVehicleDto dto);
        void DeleteVehicle(int id);
    }
}
