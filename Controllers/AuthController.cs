using backend.Dto;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto);

                if (response == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                return Ok(new {
                    Token = response.Token,
                    Role = response.Role,
                    Name = response.Name,
                    UserId = response.UserId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Login failed due to server error.",
                    detail = ex.Message
                });
            }
        }
    }
}