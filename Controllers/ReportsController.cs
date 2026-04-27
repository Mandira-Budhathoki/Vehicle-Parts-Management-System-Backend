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
    }
}