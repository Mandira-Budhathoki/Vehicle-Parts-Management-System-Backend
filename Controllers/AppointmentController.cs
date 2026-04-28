using backend.Data;
using backend.Dto;
using backend.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backend.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments/user/{userId}
        // Returns all appointments for a customer 
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Vehicle)
                .Where(a => a.Vehicle.UserId == userId)
                .OrderByDescending(a => a.Date)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    VehicleId = a.VehicleId,
                    VehicleNumber = a.Vehicle.VehicleNumber,
                    Brand = a.Vehicle.Brand,
                    Model = a.Vehicle.Model,
                    Date = a.Date,
                    Status = a.Status,
                    ServiceType = a.ServiceType,
                    Notes = a.Notes
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // POST: api/appointments
        // Book an appointment 
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            // Validate the vehicle belongs to the logged-in user
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);

            if (vehicle == null || vehicle.UserId != userId)
                return BadRequest(new { message = "Invalid vehicle selected." });

            if (dto.Date < DateTime.UtcNow)
                return BadRequest(new { message = "Appointment date must be in the future." });

            var appointment = new Appointment
            {
                VehicleId = dto.VehicleId,
                Date = dto.Date,
                ServiceType = dto.ServiceType,
                Notes = dto.Notes ?? string.Empty,
                Status = "Pending"
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment booked successfully.", appointmentId = appointment.AppointmentId });
        }

        // DELETE: api/appointments/{id}
        // Cancel an appointment 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var appointment = await _context.Appointments
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound(new { message = "Appointment not found." });

            if (appointment.Vehicle.UserId != userId)
                return Forbid();

            if (appointment.Status == "Completed")
                return BadRequest(new { message = "Cannot cancel a completed appointment." });

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(new { message = "Appointment cancelled successfully." });
        }
    }
}