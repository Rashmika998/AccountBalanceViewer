using AccountBalanceViewer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;

        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetTotalBalances()
        {
            var totalBalances = await _balanceService.GetTotalBalances();

            return Ok(totalBalances);
        }

    }
}
