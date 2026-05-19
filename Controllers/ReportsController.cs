using backend.Dto;
using backend.Services;
using backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Authorize(Roles = "STAFF,ADMIN")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        [HttpGet("regular-customers")]
        public async Task<ActionResult<IEnumerable<RegularCustomerDto>>> GetRegularCustomers()
        {
            try
            {
                var result = await _reportsService.GetRegularCustomersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch regular customers report.", detail = ex.Message });
            }
        }

        [HttpGet("high-spenders")]
        public async Task<ActionResult<IEnumerable<HighSpenderDto>>> GetHighSpenders()
        {
            try
            {
                var result = await _reportsService.GetHighSpendersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch high spenders report.", detail = ex.Message });
            }
        }

        [HttpGet("pending-credits")]
        public async Task<ActionResult<IEnumerable<PendingCreditDto>>> GetPendingCredits()
        {
            try
            {
                var result = await _reportsService.GetPendingCreditsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch pending credits report.", detail = ex.Message });
            }
        }

        [HttpGet("financial")]
        public async Task<ActionResult<FinancialReportDto>> GetFinancialReport([FromQuery] string period = "monthly", [FromQuery] string? date = null)
        {
            try
            {
                DateTime refDate = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(date))
                {
                    if (date.Length == 4 && int.TryParse(date, out int year))
                    {
                        refDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    }
                    else if (DateTime.TryParse(date, out DateTime parsedDate))
                    {
                        refDate = parsedDate;
                    }
                }
                
                var result = await _reportsService.GetFinancialReportAsync(period, refDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch financial report.", detail = ex.Message });
            }
        }
    }
}