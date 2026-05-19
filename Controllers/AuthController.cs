using backend.Dto;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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

        [Authorize]
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordDto dto, [FromServices] backend.Data.AppDbContext dbContext)
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out int userId))
                return Unauthorized();

            var user = dbContext.Users.Find(userId);
            if (user == null || user.Password != dto.CurrentPassword)
                return BadRequest(new { message = "Invalid current password." });

            user.Password = dto.NewPassword;
            dbContext.SaveChanges();

            return Ok(new { message = "Password updated successfully." });
        }
    }
}