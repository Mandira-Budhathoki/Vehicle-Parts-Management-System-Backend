using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartsController : ControllerBase
    {
        private readonly IPartsService _partsService;

        public PartsController(IPartsService partsService)
        {
            _partsService = partsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartDto>>> GetParts()
        {
            var parts = await _partsService.GetAllPartsAsync();
            return Ok(parts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartDto>> GetPart(int id)
        {
            var part = await _partsService.GetPartByIdAsync(id);
            if (part == null) return NotFound();
            return Ok(part);
        }

        [HttpPost]
        public async Task<ActionResult<PartDto>> CreatePart(PartDto partDto)
        {
            var createdPart = await _partsService.CreatePartAsync(partDto);
            return CreatedAtAction(nameof(GetPart), new { id = createdPart.PartId }, createdPart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id, PartDto partDto)
        {
            try
            {
                await _partsService.UpdatePartAsync(id, partDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            try
            {
                await _partsService.DeletePartAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
