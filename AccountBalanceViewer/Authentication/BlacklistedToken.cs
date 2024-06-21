using System.ComponentModel.DataAnnotations;

namespace AccountBalanceViewer.Authentication
{
    public class BlacklistedToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
