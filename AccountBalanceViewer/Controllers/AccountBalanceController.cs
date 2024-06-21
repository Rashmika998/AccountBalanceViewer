using AccountBalanceViewer.Authentication;
using AccountBalanceViewer.Models;
using AccountBalanceViewer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountBalanceController : ControllerBase
    {
       
        private readonly IAccountBalanceService _accountBalanceService;

        public AccountBalanceController(IAccountBalanceService accountBalanceService)
        {
            _accountBalanceService = accountBalanceService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAccountBalances()
        {
            var balance = await _accountBalanceService.GetAccountBalances();

            return Ok(balance);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetAccountBalance(int id)
        {
            var balance = await _accountBalanceService.GetAccountBalance(id);
            if(balance == null)
            {
                return NotFound("Balance not found");
            }

            return Ok(balance);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<List<AccountBalance>>> AddAccountBalance(AccountBalance balance)
        {
            return Ok(await _accountBalanceService.AddAccountBalance(balance));
        }

        [HttpPut]
        [Authorize(Roles =UserRoles.Admin)]
        public async Task<ActionResult<List<AccountBalance>>> UpdateAccountBalance(AccountBalance updatedBalance)
        {
            var status = await _accountBalanceService.UpdateAccountBalance(updatedBalance);
            if(status == StatusCodes.Status404NotFound)
            {
                return NotFound("Balance not found");
            }

            return Ok(await GetAccountBalances());
        }

        [HttpDelete]
        public async Task<ActionResult<List<AccountBalance>>> DeleteAccountBalance(int id)
        {
            var status = await _accountBalanceService.DeleteAccountBalance(id);
            if (status == StatusCodes.Status404NotFound)
            {
                return NotFound("Balance not found");
            }

            return Ok(await GetAccountBalances());
        }
    }
}
