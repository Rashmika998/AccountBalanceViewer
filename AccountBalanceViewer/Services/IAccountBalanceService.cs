using AccountBalanceViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViewer.Services
{
    public interface IAccountBalanceService
    {
        Task<ActionResult<List<AccountBalance>>> GetAccountBalances();

        Task<ActionResult<AccountBalance>> GetAccountBalance(int id);

        Task<ActionResult<List<AccountBalance>>> AddAccountBalance(AccountBalance balance);

        Task<int> UpdateAccountBalance(AccountBalance updatedBalance);

        Task<int> DeleteAccountBalance(int id);


    }
}
