using backend.Dto;
using backend.Model;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        // GET: api/staff
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(staff);
        }

        // GET: api/staff/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetStaff(int id)
        {
            var user = await _staffService.GetStaffByIdAsync(id);

            if (user == null)
            {
                return NotFound("Staff member not found.");
            }

            return Ok(user);
        }

        // POST: api/staff/register
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterStaff([FromBody] StaffRegisterDto registerDto)
        {
            try
            {
                var user = await _staffService.RegisterStaffAsync(registerDto);
                return CreatedAtAction(nameof(GetStaff), new { id = user.UserId }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/staff/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffUpdateDto updateDto)
        {
            var result = await _staffService.UpdateStaffAsync(id, updateDto);

            if (!result)
            {
                return NotFound("Staff member not found or update failed.");
            }

            return NoContent();
        }

        // DELETE: api/staff/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);

            if (!result)
            {
                return NotFound("Staff member not found.");
            }

            return NoContent();
        }
    }
}
