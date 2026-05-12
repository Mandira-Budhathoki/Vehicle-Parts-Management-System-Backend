using backend.Dto;
using backend.Model;

namespace backend.Services
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto> GetVendorByIdAsync(int id);
        Task<VendorDto> CreateVendorAsync(VendorDto vendorDto);
        Task<bool> UpdateVendorAsync(int id, VendorDto vendorDto);
        Task<bool> DeleteVendorAsync(int id);
    }
}
