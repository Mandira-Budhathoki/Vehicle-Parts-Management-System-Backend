using Microsoft.AspNetCore.Mvc;
using backend.Dto;
using backend.Interfaces;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalesInvoice([FromBody] CreateSalesDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sales = await _salesService.CreateSalesInvoiceAsync(dto);
            return CreatedAtAction(nameof(GetSalesInvoiceById), new { id = sales.SalesId }, sales);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesInvoices()
        {
            var sales = await _salesService.GetAllSalesInvoicesAsync();
            return Ok(sales);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesInvoiceById(int id)
        {
            var sales = await _salesService.GetSalesInvoiceByIdAsync(id);
            if (sales == null) return NotFound();
            return Ok(sales);
        }
    }
}