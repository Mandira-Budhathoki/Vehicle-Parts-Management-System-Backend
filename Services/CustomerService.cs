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

