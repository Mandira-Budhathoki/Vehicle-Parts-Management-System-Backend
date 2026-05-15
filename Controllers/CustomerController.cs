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

        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto dto)
        {
            _service.Register(dto);
            return Ok(new { message = "User registered successfully" });
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
    }
}