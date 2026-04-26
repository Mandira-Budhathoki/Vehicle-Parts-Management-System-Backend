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
            _service.AddVehicle(userId, dto);
            return Ok("Vehicle added");
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetVehicles(int userId)
        {
            return Ok(_service.GetVehicles(userId));
        }

        [HttpPut("{id}")]
        public IActionResult UpdateVehicle(int id, CreateVehicleDto dto)
        {
            _service.UpdateVehicle(id, dto);
            return Ok("Vehicle updated");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteVehicle(int id)
        {
            _service.DeleteVehicle(id);
            return Ok("Vehicle deleted");
        }
    }
}
