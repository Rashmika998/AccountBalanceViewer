using AccountBalanceViewer.Data;
using AccountBalanceViewer.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountBalanceViewer.Services
{
    public class BalanceService:IBalanceService
    {
        private readonly DataContext _context;

        public BalanceService(DataContext context)
        {
            _context = context;
        }
        public async Task<AccountBalance> GetTotalBalances()
        {
            var totalRnD = await _context.AccountBalances.SumAsync(ab => ab.RnD);
            var totalCanteen = await _context.AccountBalances.SumAsync(ab => ab.Canteen);
            var totalCEOCarExpenses = await _context.AccountBalances.SumAsync(ab => ab.CEOCarExpenses);
            var totalMarketing = await _context.AccountBalances.SumAsync(ab => ab.Marketing);
            var totalParkingFines = await _context.AccountBalances.SumAsync(ab => ab.ParkingFines);

            var totalBalances = new AccountBalance
            {
                RnD = totalRnD,
                Canteen = totalCanteen,
                CEOCarExpenses = totalCEOCarExpenses,
                Marketing = totalMarketing,
                ParkingFines = totalParkingFines
            };

            return totalBalances;
        }
    }
}
