using AccountBalanceViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace AccountBalanceViewer.Services
{
    public interface IBalanceService
    {
        Task<AccountBalance> GetTotalBalances();
    }
}
