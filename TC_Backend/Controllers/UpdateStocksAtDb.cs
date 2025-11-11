using Microsoft.AspNetCore.Mvc;
using TC_Backend.BackgroundServices;

namespace TC_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateStocksAtDbController : ControllerBase
    {
        private readonly FinancialDataUpdService _financialDataService;
        private readonly ILogger<UpdateStocksAtDbController> _logger;

        public UpdateStocksAtDbController(FinancialDataUpdService financialDataService, ILogger<UpdateStocksAtDbController> logger)
        {
            _financialDataService = financialDataService;
            _logger = logger;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshFinancialData()
        {
            try
            {
                await _financialDataService.TriggerUpdateAsync();
                return Ok(new { Message = "Financial data refresh triggered successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while triggering financial data refresh.");
                return StatusCode(500, new { Error = "Failed to refresh financial data." });
            }
        }
    }
}
