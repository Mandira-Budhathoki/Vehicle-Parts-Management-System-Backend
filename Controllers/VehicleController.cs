using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehicleController(IVehicleService service)
        {
            _service = service;
        }

        [HttpPost("{userId}")]
        public IActionResult AddVehicle(int userId, CreateVehicleDto dto)
        {
            try
            {
                _service.AddVehicle(userId, dto);
                return Ok(new { message = "Vehicle added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to add vehicle: " + ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetVehicles(int userId)
        {
            return Ok(_service.GetVehicles(userId));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVehicle(int id, CreateVehicleDto dto)
        {
            try
            {
                _service.UpdateVehicle(id, dto);
                return Ok(new { message = "Vehicle updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update vehicle: " + ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            try
            {
                _service.DeleteVehicle(id);
                return Ok(new { message = "Vehicle deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to delete vehicle: " + ex.Message });
            }
        }
    }
}