using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _service.GetAllCustomersAsync();
            return Ok(customers);
        }

        [HttpGet("all-users-debug")]
        public IActionResult GetUsersDebug()
        {
            var dbOptions = new DbContextOptionsBuilder<backend.Data.AppDbContext>()
                .UseNpgsql("Host=localhost;Port=5433;Database=VehicleDB;Username=postgres;Password=messi");
            using var context = new backend.Data.AppDbContext(dbOptions.Options);
            var users = context.Users.Select(u => new { u.Email, u.Password, u.Role, u.Name }).ToList();
            return Ok(users);
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto dto)
        {
            try
            {
                int userId = _service.Register(dto);
                return Ok(new { message = "User registered successfully", userId = userId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Registration failed. The email may already be registered in the system." });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _service.Login(dto.Email, dto.Password);
            if (user == null) return Unauthorized(new { message = "Invalid email or password" });
            return Ok(user);
        }

        [HttpGet("{id}")]
        public IActionResult GetProfile(int id)
        {
            var user = _service.GetProfile(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProfile(int id, RegisterUserDto dto)
        {
            _service.UpdateProfile(id, dto);
            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            _service.DeleteCustomer(id);
            return Ok(new { message = "Customer deleted successfully" });
        }
    }
}

