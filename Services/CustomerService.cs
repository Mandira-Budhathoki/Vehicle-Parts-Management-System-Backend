namespace backend.Services
{
    using backend.Data;
    using backend.Dto;
    using backend.Interfaces;
    using backend.Model;

    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;

        public CustomerService(AppDbContext context)
        {
            _context = context;
        }

        public void Register(RegisterUserDto dto)
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
                Phone = user.Phone
            };
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
    }
}
