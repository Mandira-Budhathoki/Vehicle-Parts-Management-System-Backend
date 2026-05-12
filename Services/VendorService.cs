using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class VendorService : IVendorService
    {
        private readonly AppDbContext _context;

        public VendorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            return await _context.Vendors
                .Select(v => new VendorDto
                {
                    VendorId = v.VendorId,
                    VendorName = v.VendorName,
                    Phone = v.Phone,
                    Email = v.Email,
                    Address = v.Address
                })
                .ToListAsync();
        }

        public async Task<VendorDto> GetVendorByIdAsync(int id)
        {
            var v = await _context.Vendors.FindAsync(id);
            if (v == null) return null;

            return new VendorDto
            {
                VendorId = v.VendorId,
                VendorName = v.VendorName,
                Phone = v.Phone,
                Email = v.Email,
                Address = v.Address
            };
        }

        public async Task<VendorDto> CreateVendorAsync(VendorDto vendorDto)
        {
            var vendor = new Vendor
            {
                VendorName = vendorDto.VendorName,
                Phone = vendorDto.Phone,
                Email = vendorDto.Email,
                Address = vendorDto.Address
            };

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            
            vendorDto.VendorId = vendor.VendorId;
            return vendorDto;
        }

        public async Task<bool> UpdateVendorAsync(int id, VendorDto vendorDto)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return false;
            }

            vendor.VendorName = vendorDto.VendorName;
            vendor.Phone = vendorDto.Phone;
            vendor.Email = vendorDto.Email;
            vendor.Address = vendorDto.Address;

            _context.Vendors.Update(vendor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
            {
                return false;
            }

            _context.Vendors.Remove(vendor);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
