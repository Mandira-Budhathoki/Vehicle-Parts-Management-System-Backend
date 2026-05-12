using backend.Model;
using backend.Dto;

namespace backend.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<User>> GetAllStaffAsync();
        Task<User?> GetStaffByIdAsync(int id);
        Task<User> RegisterStaffAsync(StaffRegisterDto registerDto);
        Task<bool> UpdateStaffAsync(int id, StaffUpdateDto updateDto);
        Task<bool> DeleteStaffAsync(int id);
    }
}
