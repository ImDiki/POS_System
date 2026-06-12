using System.Windows;

namespace OICPOSレジ_2C29KS.Models
{
   
    public interface IPaymentMethod
    {
        string Name { get; }
        bool ProcessPayment(int totalAmount, int paidAmount, out int change);
    }

    
    public class CashPayment : IPaymentMethod
    {
        public string Name => "Cash";

        public bool ProcessPayment(int totalAmount, int paidAmount, out int change)
        {
            if (paidAmount < totalAmount)
            {
                MessageBox.Show("お預かり金額が足りません.", "エーラ", MessageBoxButton.OK, MessageBoxImage.Error);
                change = 0;
                return false;
            }
            change = paidAmount - totalAmount;
            return true;
        }
    }

    public class CashlessPayment : IPaymentMethod
    {
        public string Name { get; private set; }

        public CashlessPayment(string name)
        {
            Name = name;
        }

        public bool ProcessPayment(int totalAmount, int paidAmount, out int change)
        {
           
            change = 0;
            MessageBox.Show($"{Name} での決済が完了しました。\n(会計終了しました)", "Payment Success", MessageBoxButton.OK, MessageBoxImage.Information);
            return true;
        }
    }
}