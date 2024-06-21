using System.ComponentModel.DataAnnotations;

namespace AccountBalanceViewer.Models
{
    public class AccountBalance
    {
        [Key]
        public int Id { get; set; }
        public double RnD { get; set; } = 0;
        public double Canteen { get; set; } = 0;
        public double CEOCarExpenses { get; set; } = 0;
        public double Marketing { get; set; } = 0;
        public double ParkingFines { get; set; } = 0;
    }
}
