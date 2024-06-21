
using System.ComponentModel.DataAnnotations;

namespace AccountBalanceViewer.Models
{
    public class User
    {
        [Key]
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string UserRole { get; set; }
    }
}
