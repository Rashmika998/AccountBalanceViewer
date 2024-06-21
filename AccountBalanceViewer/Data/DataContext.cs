using AccountBalanceViewer.Authentication;
using AccountBalanceViewer.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountBalanceViewer.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 
        }

        public DbSet<AccountBalance> AccountBalances { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    }
}
