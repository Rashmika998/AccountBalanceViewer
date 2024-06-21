using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountBalanceViewer.Services
{
    public sealed class AccountBalanceService:IAccountBalanceService
    {
        private readonly DataContext _context;

        public AccountBalanceService(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<List<AccountBalance>>> GetAccountBalances()
        {
            var balance = await _context.AccountBalances.ToListAsync();

            return balance;
        }

        public async Task<ActionResult<AccountBalance>> GetAccountBalance(int id)
        {
            var balance = await _context.AccountBalances.FindAsync(id);
            return balance;
        }

        public async Task<ActionResult<List<AccountBalance>>> AddAccountBalance(AccountBalance balance)
        {
            _context.AccountBalances.Add(balance);
            await _context.SaveChangesAsync();

            return await GetAccountBalances();
        }

        public async Task<int> UpdateAccountBalance(AccountBalance updatedBalance)
        {
            var balance = await _context.AccountBalances.FindAsync(updatedBalance.Id);
            if (balance == null)
                return StatusCodes.Status404NotFound;

            balance.RnD = updatedBalance.RnD;
            balance.CEOCarExpenses = updatedBalance.CEOCarExpenses;
            balance.Marketing = updatedBalance.Marketing;
            balance.ParkingFines = updatedBalance.ParkingFines;

            await _context.SaveChangesAsync();

            return StatusCodes.Status200OK;
        }

        public async Task<int> DeleteAccountBalance(int id)
        {
            var balance = await _context.AccountBalances.FindAsync(id);
            if (balance == null)
                return StatusCodes.Status404NotFound;

            _context.AccountBalances.Remove(balance);

            await _context.SaveChangesAsync();

            return StatusCodes.Status200OK;
        }
    }
}
