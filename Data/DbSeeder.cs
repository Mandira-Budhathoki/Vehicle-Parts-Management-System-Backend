using backend.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(AppDbContext context)
        {
            // 1. Seed Customer User if none exists
            var customer = await context.Users.FirstOrDefaultAsync(u => u.Role.ToUpper() == "CUSTOMER");
            if (customer == null)
            {
                customer = new User
                {
                    Name = "John Doe",
                    Email = "jhon@gmail.com", // align with default customer email
                    Password = "jhon",
                    Role = "CUSTOMER",
                    Phone = "1234567890",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(customer);
                await context.SaveChangesAsync();
            }

            // 2. Seed Staff User if none exists
            var staff = await context.Users.FirstOrDefaultAsync(u => u.Role.ToUpper() == "STAFF" || u.Role.ToUpper() == "ADMIN");
            if (staff == null)
            {
                staff = new User
                {
                    Name = "Jane Staff",
                    Email = "staff@gmail.com",
                    Password = "staff",
                    Role = "STAFF",
                    Phone = "0987654321",
                    CreatedAt = DateTime.UtcNow
                };
                context.Users.Add(staff);
                await context.SaveChangesAsync();
            }

            // 3. Seed Vendor if none exists
            var vendor = await context.Vendors.FirstOrDefaultAsync();
            if (vendor == null)
            {
                vendor = new Vendor
                {
                    VendorName = "Premium Auto Parts Distributors",
                    Phone = "555-0199",
                    Email = "sales@premiumparts.com",
                    Address = "100 Industrial Parkway"
                };
                context.Vendors.Add(vendor);
                await context.SaveChangesAsync();
            }

            // 4. Seed Parts if none exists
            var parts = new List<Part>();
            if (!await context.Parts.AnyAsync())
            {
                parts = new List<Part>
                {
                    new Part { PartName = "Ceramic Brake Pads", Category = "Braking", Description = "High durability brake pads", Price = 89.99m, StockQuantity = 120, ReorderLevel = 10, VendorId = vendor.VendorId },
                    new Part { PartName = "Platinum Spark Plug", Category = "Ignition", Description = "Long-life spark plugs", Price = 12.50m, StockQuantity = 350, ReorderLevel = 30, VendorId = vendor.VendorId },
                    new Part { PartName = "Synthetic Engine Oil 5W-30", Category = "Maintenance", Description = "Full synthetic motor oil 5L", Price = 45.00m, StockQuantity = 80, ReorderLevel = 15, VendorId = vendor.VendorId },
                    new Part { PartName = "12V Car Battery", Category = "Electrical", Description = "Maintenance-free lead acid battery", Price = 129.99m, StockQuantity = 45, ReorderLevel = 5, VendorId = vendor.VendorId },
                    new Part { PartName = "H7 Headlight Bulb", Category = "Lighting", Description = "Super bright halogen bulb", Price = 18.75m, StockQuantity = 200, ReorderLevel = 20, VendorId = vendor.VendorId }
                };
                context.Parts.AddRange(parts);
                await context.SaveChangesAsync();
            }
            else
            {
                parts = await context.Parts.ToListAsync();
            }

            // 5. Seed Sales & SalesItems if none exist
            if (!await context.Sales.AnyAsync())
            {
                var today = DateTime.UtcNow;

                var sale1 = new Sales
                {
                    UserId = customer.UserId,
                    StaffId = staff.UserId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 10, 30, 0, DateTimeKind.Utc),
                    Discount = 0,
                    TotalAmount = 202.48m,
                    FinalAmount = 202.48m,
                    PaymentStatus = "PAID",
                    SalesItems = new List<SalesItem>
                    {
                        new SalesItem { PartId = parts[0].PartId, Quantity = 2, Price = parts[0].Price, Subtotal = parts[0].Price * 2 },
                        new SalesItem { PartId = parts[4].PartId, Quantity = 1, Price = parts[4].Price, Subtotal = parts[4].Price }
                    }
                };

                var sale2 = new Sales
                {
                    UserId = customer.UserId,
                    StaffId = staff.UserId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 14, 15, 0, DateTimeKind.Utc).AddDays(-2),
                    Discount = 10m,
                    TotalAmount = 142.49m,
                    FinalAmount = 132.49m,
                    PaymentStatus = "PAID",
                    SalesItems = new List<SalesItem>
                    {
                        new SalesItem { PartId = parts[1].PartId, Quantity = 1, Price = parts[1].Price, Subtotal = parts[1].Price },
                        new SalesItem { PartId = parts[3].PartId, Quantity = 1, Price = parts[3].Price, Subtotal = parts[3].Price }
                    }
                };

                var sale3 = new Sales
                {
                    UserId = customer.UserId,
                    StaffId = staff.UserId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 9, 0, 0, DateTimeKind.Utc).AddDays(-5),
                    Discount = 0,
                    TotalAmount = 90.00m,
                    FinalAmount = 90.00m,
                    PaymentStatus = "PENDING",
                    SalesItems = new List<SalesItem>
                    {
                        new SalesItem { PartId = parts[2].PartId, Quantity = 2, Price = parts[2].Price, Subtotal = parts[2].Price * 2 }
                    }
                };

                var sale4 = new Sales
                {
                    UserId = customer.UserId,
                    StaffId = staff.UserId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 16, 45, 0, DateTimeKind.Utc).AddDays(-8),
                    Discount = 5m,
                    TotalAmount = 114.99m,
                    FinalAmount = 109.99m,
                    PaymentStatus = "PAID",
                    SalesItems = new List<SalesItem>
                    {
                        new SalesItem { PartId = parts[1].PartId, Quantity = 2, Price = parts[1].Price, Subtotal = parts[1].Price * 2 },
                        new SalesItem { PartId = parts[0].PartId, Quantity = 1, Price = parts[0].Price, Subtotal = parts[0].Price }
                    }
                };

                var sale5 = new Sales
                {
                    UserId = customer.UserId,
                    StaffId = staff.UserId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 11, 20, 0, DateTimeKind.Utc).AddDays(-12),
                    Discount = 0,
                    TotalAmount = 37.50m,
                    FinalAmount = 37.50m,
                    PaymentStatus = "PAID",
                    SalesItems = new List<SalesItem>
                    {
                        new SalesItem { PartId = parts[4].PartId, Quantity = 2, Price = parts[4].Price, Subtotal = parts[4].Price * 2 }
                    }
                };

                context.Sales.AddRange(sale1, sale2, sale3, sale4, sale5);
                await context.SaveChangesAsync();
            }

            // 6. Seed Purchases & PurchaseItems if none exist
            if (!await context.Purchases.AnyAsync())
            {
                var today = DateTime.UtcNow;

                var purchase1 = new Purchase
                {
                    UserId = staff.UserId,
                    VendorId = vendor.VendorId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 8, 30, 0, DateTimeKind.Utc).AddDays(-1),
                    TotalAmount = 500.00m,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { PartId = parts[0].PartId, Quantity = 10, CostPrice = 50.00m, Subtotal = 500.00m }
                    }
                };

                var purchase2 = new Purchase
                {
                    UserId = staff.UserId,
                    VendorId = vendor.VendorId,
                    Date = new DateTime(today.Year, today.Month, today.Day, 13, 0, 0, DateTimeKind.Utc).AddDays(-6),
                    TotalAmount = 300.00m,
                    PurchaseItems = new List<PurchaseItem>
                    {
                        new PurchaseItem { PartId = parts[2].PartId, Quantity = 10, CostPrice = 30.00m, Subtotal = 300.00m }
                    }
                };

                context.Purchases.AddRange(purchase1, purchase2);
                await context.SaveChangesAsync();
            }
        }
    }
}
