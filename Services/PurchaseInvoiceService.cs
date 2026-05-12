using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly AppDbContext _context;

        public PurchaseInvoiceService(AppDbContext context)
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
                    Email = v.Email
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetAllPartsAsync()
        {
            return await _context.Parts
                .Select(p => new {
                    PartId = p.PartId,
                    PartName = p.PartName,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseInvoiceDto>> GetAllPurchaseInvoicesAsync()
        {
            return await _context.Purchases
                .Include(p => p.Vendor)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Part)
                .OrderByDescending(p => p.Date)
                .Select(p => new PurchaseInvoiceDto
                {
                    PurchaseId = p.PurchaseId,
                    VendorId = p.VendorId,
                    VendorName = p.Vendor.VendorName,
                    Date = p.Date,
                    TotalAmount = p.TotalAmount,
                    Items = p.PurchaseItems.Select(pi => new PurchaseItemDto
                    {
                        PurchaseItemId = pi.PurchaseItemId,
                        PartId = pi.PartId,
                        PartName = pi.Part.PartName,
                        Quantity = pi.Quantity,
                        CostPrice = pi.CostPrice,
                        Subtotal = pi.Subtotal
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<PurchaseInvoiceDto> CreatePurchaseInvoiceAsync(int adminUserId, CreatePurchaseInvoiceDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verify vendor exists
                var vendor = await _context.Vendors.FindAsync(dto.VendorId);
                if (vendor == null) throw new Exception("Vendor not found.");

                // Calculate total amount and prepare items
                decimal totalAmount = 0;
                var purchaseItems = new List<PurchaseItem>();

                foreach (var itemDto in dto.Items)
                {
                    var part = await _context.Parts.FindAsync(itemDto.PartId);
                    if (part == null) throw new Exception($"Part with ID {itemDto.PartId} not found.");

                    var subtotal = itemDto.Quantity * itemDto.CostPrice;
                    totalAmount += subtotal;

                    purchaseItems.Add(new PurchaseItem
                    {
                        PartId = itemDto.PartId,
                        Quantity = itemDto.Quantity,
                        CostPrice = itemDto.CostPrice,
                        Subtotal = subtotal
                    });

                    // Critical step: Update the stock quantity dynamically!
                    part.StockQuantity += itemDto.Quantity;
                }

                // Create the purchase invoice
                var purchase = new Purchase
                {
                    UserId = adminUserId,
                    VendorId = dto.VendorId,
                    Date = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    PurchaseItems = purchaseItems
                };

                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                // Commit the transaction to ensure all operations (including stock updates) succeed atomically
                await transaction.CommitAsync();

                // Return the newly created invoice DTO
                return await GetPurchaseInvoiceByIdAsync(purchase.PurchaseId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<PurchaseInvoiceDto> GetPurchaseInvoiceByIdAsync(int id)
        {
            var p = await _context.Purchases
                .Include(p => p.Vendor)
                .Include(p => p.PurchaseItems)
                    .ThenInclude(pi => pi.Part)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (p == null) return null;

            return new PurchaseInvoiceDto
            {
                PurchaseId = p.PurchaseId,
                VendorId = p.VendorId,
                VendorName = p.Vendor.VendorName,
                Date = p.Date,
                TotalAmount = p.TotalAmount,
                Items = p.PurchaseItems.Select(pi => new PurchaseItemDto
                {
                    PurchaseItemId = pi.PurchaseItemId,
                    PartId = pi.PartId,
                    PartName = pi.Part.PartName,
                    Quantity = pi.Quantity,
                    CostPrice = pi.CostPrice,
                    Subtotal = pi.Subtotal
                }).ToList()
            };
        }
    }
}
