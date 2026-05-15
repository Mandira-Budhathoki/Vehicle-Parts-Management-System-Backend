using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto dto)
        {
            int userId = _service.Register(dto);
            return Ok(new { message = "User registered successfully", userId = userId });
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