using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.Dto;
using backend.Interfaces;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class SalesService : ISalesService
    {
        private readonly AppDbContext _context;

        public SalesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalesDto> CreateSalesInvoiceAsync(CreateSalesDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var salesItems = new List<SalesItem>();
                decimal totalAmount = 0;

                foreach (var itemDto in dto.SalesItems)
                {
                    var part = await _context.Parts.FindAsync(itemDto.PartId);
                    if (part == null)
                        throw new Exception($"Part with ID {itemDto.PartId} not found.");

                    if (part.StockQuantity < itemDto.Quantity)
                        throw new Exception($"Insufficient stock for {part.PartName}. Available: {part.StockQuantity}, Requested: {itemDto.Quantity}");

                    // Deduct stock
                    part.StockQuantity -= itemDto.Quantity;

                    var subtotal = part.Price * itemDto.Quantity;
                    totalAmount += subtotal;

                    salesItems.Add(new SalesItem
                    {
                        PartId = itemDto.PartId,
                        Quantity = itemDto.Quantity,
                        Price = part.Price,
                        Subtotal = subtotal
                    });
                }

                // Discount logic will be implemented by another team member in Feature 16
                decimal discountAmount = 0;
                var finalAmount = totalAmount - discountAmount;

                var sales = new Sales
                {
                    UserId = dto.UserId,
                    StaffId = dto.StaffId,
                    Date = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Discount = discountAmount,
                    FinalAmount = finalAmount,
                    PaymentStatus = string.IsNullOrWhiteSpace(dto.PaymentStatus) ? "Completed" : dto.PaymentStatus,
                    SalesItems = salesItems
                };

                _context.Sales.Add(sales);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await GetSalesInvoiceByIdAsync(sales.SalesId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<SalesDto>> GetAllSalesInvoicesAsync()
        {
            var salesList = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SalesItems)
                .ThenInclude(si => si.Part)
                .OrderByDescending(s => s.Date)
                .ToListAsync();

            return salesList.Select(MapToDto);
        }

        public async Task<SalesDto> GetSalesInvoiceByIdAsync(int id)
        {
            var sales = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SalesItems)
                .ThenInclude(si => si.Part)
                .FirstOrDefaultAsync(s => s.SalesId == id);

            if (sales == null) return null;

            return MapToDto(sales);
        }

        private SalesDto MapToDto(Sales sales)
        {
            return new SalesDto
            {
                SalesId = sales.SalesId,
                UserId = sales.UserId,
                CustomerName = sales.User?.Name,
                StaffId = sales.StaffId,
                Date = sales.Date,
                TotalAmount = sales.TotalAmount,
                Discount = sales.Discount,
                FinalAmount = sales.FinalAmount,
                PaymentStatus = sales.PaymentStatus,
                SalesItems = sales.SalesItems?.Select(si => new SalesItemDto
                {
                    SalesItemId = si.SalesItemId,
                    SalesId = si.SalesId,
                    PartId = si.PartId,
                    PartName = si.Part?.PartName,
                    Quantity = si.Quantity,
                    Price = si.Price,
                    Subtotal = si.Subtotal
                }).ToList() ?? new List<SalesItemDto>()
            };
        }
    }
}
