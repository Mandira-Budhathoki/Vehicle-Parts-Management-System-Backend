using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/staff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetStaff()
        {
            return await _context.Users
                .Where(u => u.Role == "STAFF" || u.Role == "ADMIN")
                .ToListAsync();
        }

        // GET: api/staff/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetStaff(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return NotFound("Staff member not found.");
            }

            return user;
        }

        // POST: api/staff/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterStaff([FromBody] StaffRegisterDto registerDto)
        {
            if (_context.Users.Any(u => u.Email == registerDto.Email))
            {
                return BadRequest("Email already exists.");
            }

            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = registerDto.Password, // In real app, hash this
                Role = registerDto.Role,
                Phone = registerDto.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStaff), new { id = user.UserId }, user);
        }

        // PUT: api/staff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffUpdateDto updateDto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return NotFound("Staff member not found.");
            }

            user.Name = updateDto.Name;
            user.Email = updateDto.Email;
            user.Phone = updateDto.Phone;
            user.Role = updateDto.Role;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/staff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || (user.Role != "STAFF" && user.Role != "ADMIN"))
            {
                return NotFound("Staff member not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
