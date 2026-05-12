using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PartsService : IPartsService
    {
        private readonly AppDbContext _context;

        public PartsService(AppDbContext context)
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
                    Category = p.Category,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ReorderLevel = p.ReorderLevel,
                    VendorId = p.VendorId
                })
                .ToListAsync();
        }

        public async Task<PartDto?> GetPartByIdAsync(int id)
        {
            var p = await _context.Parts.FindAsync(id);
            if (p == null) return null;

            return new PartDto
            {
                PartId = p.PartId,
                PartName = p.PartName,
                Category = p.Category,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                ReorderLevel = p.ReorderLevel,
                VendorId = p.VendorId
            };
        }

        public async Task<PartDto> CreatePartAsync(PartDto dto)
        {
            var part = new Part
            {
                PartName = dto.PartName,
                Category = dto.Category,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel,
                VendorId = dto.VendorId
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            dto.PartId = part.PartId;
            return dto;
        }

        public async Task UpdatePartAsync(int id, PartDto dto)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) throw new Exception("Part not found");

            part.PartName = dto.PartName;
            part.Category = dto.Category;
            part.Description = dto.Description;
            part.Price = dto.Price;
            part.StockQuantity = dto.StockQuantity;
            part.ReorderLevel = dto.ReorderLevel;
            part.VendorId = dto.VendorId;

            await _context.SaveChangesAsync();
        }

        public async Task DeletePartAsync(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) throw new Exception("Part not found");

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
        }
    }
}
