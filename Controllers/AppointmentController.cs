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
        // Returns all appointments for a specific customer
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
        // Book an appointment (customer)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Validate vehicle belongs to logged-in user
            var vehicle = await _context.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null || vehicle.UserId != userId)
                return BadRequest(new { message = "Invalid vehicle selected." });

            if (dto.Date < DateTime.UtcNow)
                return BadRequest(new { message = "Appointment date must be in the future." });

            // ── Double-booking check: same 1-hour window ──────────────────────
            var windowStart = dto.Date.AddMinutes(-30);
            var windowEnd = dto.Date.AddMinutes(30);
            var conflict = await _context.Appointments
                .AnyAsync(a =>
                    a.Status != "Cancelled" &&
                    a.Date >= windowStart &&
                    a.Date <= windowEnd);

            if (conflict)
                return BadRequest(new { message = "That time slot is already booked. Please choose a different time (at least 30 minutes apart)." });

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
        // Cancel an appointment (customer)
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


        // GET: api/appointments/all
        // Returns all appointments for staff to review
        [HttpGet("all")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var query = _context.Appointments
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            var appointments = await query
                .OrderByDescending(a => a.Date)
                .Select(a => new StaffAppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    VehicleId = a.VehicleId,
                    VehicleNumber = a.Vehicle.VehicleNumber,
                    Brand = a.Vehicle.Brand,
                    Model = a.Vehicle.Model,
                    CustomerName = a.Vehicle.User.Name,
                    CustomerEmail = a.Vehicle.User.Email,
                    CustomerId = a.Vehicle.UserId,
                    Date = a.Date,
                    Status = a.Status,
                    ServiceType = a.ServiceType,
                    Notes = a.Notes
                })
                .ToListAsync();

            return Ok(appointments);
        }

       
        [HttpPut("{id}/status")]
        [Authorize(Roles = "STAFF,ADMIN")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            var allowed = new[] { "Approved", "Rejected", "Completed" };
            if (!allowed.Contains(dto.Status))
                return BadRequest(new { message = $"Status must be one of: {string.Join(", ", allowed)}" });

            var appointment = await _context.Appointments
                .Include(a => a.Vehicle)
                    .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound(new { message = "Appointment not found." });

            appointment.Status = dto.Status;
            await _context.SaveChangesAsync();

            // ── Write notification for the customer ───────────────────────────
            var message = dto.Status switch
            {
                "Approved" => $"Your appointment for {appointment.ServiceType} on {appointment.Date:dd MMM yyyy, hh:mm tt} has been APPROVED.",
                "Rejected" => $"Your appointment for {appointment.ServiceType} on {appointment.Date:dd MMM yyyy, hh:mm tt} has been REJECTED. Please book a new time.",
                "Completed" => $"Your appointment for {appointment.ServiceType} on {appointment.Date:dd MMM yyyy, hh:mm tt} has been marked as COMPLETED.",
                _ => string.Empty
            };

            if (!string.IsNullOrEmpty(message))
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = appointment.Vehicle.UserId,
                    Message = message,
                    Type = "Appointment",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = $"Appointment marked as {dto.Status}." });
        }
    }
}
