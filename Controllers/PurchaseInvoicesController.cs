using backend.Dto;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "admin")]
    public class PurchaseInvoicesController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _purchaseService;

        public PurchaseInvoicesController(IPurchaseInvoiceService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        // GET: api/PurchaseInvoices/vendors
        [HttpGet("vendors")]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
        {
            var vendors = await _purchaseService.GetAllVendorsAsync();
            return Ok(vendors);
        }

        // GET: api/PurchaseInvoices/parts
        [HttpGet("parts")]
        public async Task<IActionResult> GetParts()
        {
            var parts = await _purchaseService.GetAllPartsAsync();
            return Ok(parts);
        }

        // GET: api/PurchaseInvoices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInvoiceDto>>> GetInvoices()
        {
            var invoices = await _purchaseService.GetAllPurchaseInvoicesAsync();
            return Ok(invoices);
        }

        // POST: api/PurchaseInvoices
        [HttpPost]
        public async Task<ActionResult<PurchaseInvoiceDto>> CreateInvoice(CreatePurchaseInvoiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // In a real app, you might get the UserId from claims. Here we mock it or pass it.
            // Let's assume Admin User ID is 1 for the sake of this feature since we don't have user context setup here.
            int adminUserId = 1;

            try
            {
                var invoice = await _purchaseService.CreatePurchaseInvoiceAsync(adminUserId, dto);
                return CreatedAtAction(nameof(GetInvoices), new { id = invoice.PurchaseId }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
