using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IPartService _partService;

        public AdminController(IPartService partService)
        {
            _partService = partService;
        }

        // GET: api/admin/parts
        [HttpGet("parts")]
        public async Task<ActionResult<IEnumerable<PartDto>>> GetParts()
        {
            var parts = await _partService.GetAllPartsAsync();
            return Ok(parts);
        }

        // GET: api/admin/parts/5
        [HttpGet("parts/{id}")]
        public async Task<ActionResult<PartDto>> GetPart(int id)
        {
            var part = await _partService.GetPartByIdAsync(id);
            if (part == null)
            {
                return NotFound();
            }
            return Ok(part);
        }

        // POST: api/admin/parts
        [HttpPost("parts")]
        public async Task<ActionResult<PartDto>> CreatePart(CreatePartDto createPartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPart = await _partService.CreatePartAsync(createPartDto);
            return CreatedAtAction(nameof(GetPart), new { id = createdPart.PartId }, createdPart);
        }

        // PUT: api/admin/parts/5
        [HttpPut("parts/{id}")]
        public async Task<IActionResult> UpdatePart(int id, UpdatePartDto updatePartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedPart = await _partService.UpdatePartAsync(id, updatePartDto);
            if (updatedPart == null)
            {
                return NotFound();
            }

            return Ok(updatedPart);
        }

        // DELETE: api/admin/parts/5
        [HttpDelete("parts/{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var success = await _partService.DeletePartAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
