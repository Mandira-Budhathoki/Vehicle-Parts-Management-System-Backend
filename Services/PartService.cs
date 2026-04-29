using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PartService : IPartService
    {
        private readonly AppDbContext _context;

        public PartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PartDto>> GetAllPartsAsync()
        {
            return await _context.Parts
                .Select(p => new PartDto
                {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ReorderLevel = p.ReorderLevel,
                    VendorId = p.VendorId
                })
                .ToListAsync();
        }

        public async Task<PartDto> GetPartByIdAsync(int id)
        {
            var p = await _context.Parts.FindAsync(id);
            if (p == null) return null;

            return new PartDto
            {
                PartId = p.PartId,
                PartName = p.PartName,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel,
                VendorId = p.VendorId
            };
        }

        public async Task<PartDto> CreatePartAsync(CreatePartDto createPartDto)
        {
            var part = new Part
            {
                PartName = createPartDto.PartName,
                Description = createPartDto.Description,
                Price = createPartDto.Price,
                StockQuantity = createPartDto.StockQuantity,
                ReorderLevel = createPartDto.ReorderLevel,
                VendorId = createPartDto.VendorId
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            return new PartDto
            {
                PartId = part.PartId,
                PartName = part.PartName,
                Description = part.Description,
                Price = part.Price,
                StockQuantity = part.StockQuantity,
                ReorderLevel = part.ReorderLevel,
                VendorId = part.VendorId
            };
        }

        public async Task<PartDto> UpdatePartAsync(int id, UpdatePartDto updatePartDto)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return null;

            part.PartName = updatePartDto.PartName;
            part.Description = updatePartDto.Description;
            part.Price = updatePartDto.Price;
            part.StockQuantity = updatePartDto.StockQuantity;
            part.ReorderLevel = updatePartDto.ReorderLevel;
            part.VendorId = updatePartDto.VendorId;

            await _context.SaveChangesAsync();

            return new PartDto
            {
                PartId = part.PartId,
                PartName = part.PartName,
                Description = part.Description,
                Price = part.Price,
                StockQuantity = part.StockQuantity,
                ReorderLevel = part.ReorderLevel,
                VendorId = part.VendorId
            };
        }

        public async Task<bool> DeletePartAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return false;

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
