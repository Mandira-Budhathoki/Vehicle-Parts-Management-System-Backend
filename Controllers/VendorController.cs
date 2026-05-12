using backend.Dto;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ADMIN,admin")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        // GET: api/vendor/test
        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok("Vendor Controller is reachable!");
        }

        // GET: api/vendor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
        {
            var vendors = await _vendorService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        // GET: api/vendor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<VendorDto>> GetVendor(int id)
        {
            var vendor = await _vendorService.GetVendorByIdAsync(id);

            if (vendor == null)
            {
                return NotFound("Vendor not found.");
            }

            return Ok(vendor);
        }

        // POST: api/vendor
        [HttpPost]
        public async Task<ActionResult<VendorDto>> CreateVendor([FromBody] VendorDto vendorDto)
        {
            try
            {
                var vendor = await _vendorService.CreateVendorAsync(vendorDto);
                return CreatedAtAction(nameof(GetVendor), new { id = vendor.VendorId }, vendor);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/vendor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, [FromBody] VendorDto vendorDto)
        {
            var result = await _vendorService.UpdateVendorAsync(id, vendorDto);

            if (!result)
            {
                return NotFound("Vendor not found or update failed.");
            }

            return NoContent();
        }

        // DELETE: api/vendor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var result = await _vendorService.DeleteVendorAsync(id);

            if (!result)
            {
                return NotFound("Vendor not found.");
            }

            return NoContent();
        }
    }
}
