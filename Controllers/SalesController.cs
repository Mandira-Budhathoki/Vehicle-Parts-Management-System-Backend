using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "staff,admin")]
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

            try
            {
                var sales = await _salesService.CreateSalesInvoiceAsync(dto);
                return CreatedAtAction(nameof(GetSalesInvoiceById), new { id = sales.SalesId }, sales);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSalesInvoices()
        {
            try
            {
                var sales = await _salesService.GetAllSalesInvoicesAsync();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesInvoiceById(int id)
        {
            try
            {
                var sales = await _salesService.GetSalesInvoiceByIdAsync(id);
                if (sales == null) return NotFound();
                return Ok(sales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
