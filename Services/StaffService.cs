using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class StaffService : IStaffService
    {
        private readonly AppDbContext _context;

        public StaffService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllStaffAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "STAFF" || u.Role == "ADMIN")
                .ToListAsync();
        }

        public async Task<User?> GetStaffByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return null;
            }
            return user;
        }

        public async Task<User> RegisterStaffAsync(StaffRegisterDto registerDto)
        {
            if (_context.Users.Any(u => u.Email == registerDto.Email))
            {
                throw new Exception("Email already exists.");
            }

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = registerDto.Password, // Note: Should be hashed in production
                Role = registerDto.Role,
                Phone = registerDto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateStaffAsync(int id, StaffUpdateDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return false;
            }

            user.Name = updateDto.Name;
            user.Email = updateDto.Email;
            user.Phone = updateDto.Phone;
            user.Role = updateDto.Role;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.UserId == id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
