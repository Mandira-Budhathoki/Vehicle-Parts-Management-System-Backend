namespace backend.Services
{
    using backend.Data;
    using backend.Dto;
    using backend.Interfaces;
    using backend.Model;
    using Microsoft.EntityFrameworkCore;

    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public int Register(RegisterUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone,
                Role = "CUSTOMER",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Notify the user to change their default password
            var notification = new Notification
            {
                UserId = user.UserId,
                Message = "Welcome! For your security, please update your default password.",
                Type = "PASSWORD_RESET_REQUIRED",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Notifications.Add(notification);
            _context.SaveChanges();
            
            return user.UserId;
        }

        public UserDto? Login(string email, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                TotalSpent = _context.Sales.Where(s => s.UserId == user.UserId).Sum(s => (decimal?)s.FinalAmount) ?? 0
            };
        }

        public UserDto GetProfile(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return null;

            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                TotalSpent = _context.Sales.Where(s => s.UserId == user.UserId).Sum(s => (decimal?)s.FinalAmount) ?? 0
            };
        }

        public IEnumerable<UserDto> GetAllCustomers()
        {
            return _context.Users
                .Where(u => u.Role.ToUpper() == "CUSTOMER")
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    TotalSpent = _context.Sales.Where(s => s.UserId == u.UserId).Sum(s => (decimal?)s.FinalAmount) ?? 0
                }).ToList();
        }

        public void UpdateProfile(int id, RegisterUserDto dto)
        {
            var user = _context.Users.Find(id);
            if (user == null) return;

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Phone = dto.Phone;

            _context.SaveChanges();
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "CUSTOMER")
                .Include(u => u.Vehicles)
                .Select(u => new CustomerResponseDto
                {
                    UserId = u.UserId,
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    TotalSpent = _context.Sales.Where(s => s.UserId == u.UserId).Sum(s => (decimal?)s.FinalAmount) ?? 0,
                    Vehicles = u.Vehicles.Select(v => new VehicleDto
                    {
                        VehicleId = v.VehicleId,
                        VehicleNumber = v.VehicleNumber,
                        Brand = v.Brand,
                        Model = v.Model,
                        Year = v.Year
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<CustomerFullProfileDto?> GetFullCustomerProfileAsync(int id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.Role == "CUSTOMER");

            if (user == null) return null;

            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == id)
                .Select(v => new VehicleDto
                {
                    VehicleId = v.VehicleId,
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    Year = v.Year
                })
                .ToListAsync();

            var sales = await _context.Sales
                .Include(s => s.SalesItems)
                    .ThenInclude(si => si.Part)
                .Where(s => s.UserId == id)
                .OrderByDescending(s => s.Date)
                .Select(s => new SalesHistoryDto
                {
                    SalesId = s.SalesId,
                    Date = s.Date,
                    TotalAmount = s.TotalAmount,
                    Discount = s.Discount,
                    FinalAmount = s.FinalAmount,
                    PaymentStatus = s.PaymentStatus,
                    Items = s.SalesItems.Select(si => new SalesItemHistoryDto
                    {
                        PartName = si.Part != null ? si.Part.PartName : "Unknown Part",
                        Quantity = si.Quantity,
                        Price = si.Price,
                        Subtotal = si.Subtotal
                    }).ToList()
                })
                .ToListAsync();

            var appointments = await _context.Appointments
                .Include(a => a.Vehicle)
                .Where(a => a.Vehicle.UserId == id)
                .OrderByDescending(a => a.Date)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    VehicleId = a.VehicleId,
                    VehicleNumber = a.Vehicle.VehicleNumber,
                    Brand = a.Vehicle.Brand,
                    Model = a.Vehicle.Model,
                    Date = a.Date,
                    Status = a.Status,
                    ServiceType = a.ServiceType,
                    Notes = a.Notes
                })
                .ToListAsync();

            var requests = await _context.PartRequests
                .Where(r => r.UserId == id)
                .OrderByDescending(r => r.RequestDate)
                .Select(r => new PartRequestDto
                {
                    RequestId = r.RequestId,
                    PartName = r.PartName,
                    Notes = r.Part != null ? r.Part.Description : null,
                    Status = r.Status,
                    RequestDate = r.RequestDate
                })
                .ToListAsync();

            return new CustomerFullProfileDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Vehicles = vehicles,
                SalesHistory = sales,
                Appointments = appointments,
                PartRequests = requests
            };
        }

        public void DeleteCustomer(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}

