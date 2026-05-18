using backend.Dto;
using backend.Interfaces;
using backend.Model;
using backend.Data;

public class SalesService : ISalesService
{
    private readonly AppDbContext _context;

    public SalesService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Sales> CreateSaleAsync(CreateSalesDto dto)
    {
        var sale = new Sales
        {
            UserId = dto.UserId,
            StaffId = dto.StaffId,
            Date = DateTime.Now,
            TotalAmount = 0,
            Discount = 0,
            FinalAmount = 0,
            PaymentStatus = "Paid",
            SalesItems = new List<SalesItem>()
        };

        foreach (var item in dto.Items)
        {
            var part = await _context.Parts.FindAsync(item.PartId);

            if (part == null)
                throw new Exception("Part not found");

            var saleItem = new SalesItem
            {
                PartId = item.PartId,
                Quantity = item.Quantity,
                Price = part.Price,
                Subtotal = part.Price * item.Quantity
            };

            sale.TotalAmount += saleItem.Subtotal;
            sale.SalesItems.Add(saleItem);
        }

        // FEATURE 16 LOGIC (LOYALTY)
        if (sale.TotalAmount > 5000)
        {
            sale.Discount = sale.TotalAmount * 0.10m;
        }

        sale.FinalAmount = sale.TotalAmount - sale.Discount;

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        return sale;
    }
}