using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/sales")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _service;

        public SalesController(ISalesService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSale(CreateSalesDto dto)
        {
            var result = await _service.CreateSaleAsync(dto);
            return Ok(result);
        }
    }
}