using System;
using System.Collections.ObjectModel;
using System.Windows;
using OICPOSレジ_2C29KS.Models;

namespace OICPOSレジ_2C29KS.Views
{
    public partial class ReceiptWindow : Window
    {
     
        public ReceiptWindow(ObservableCollection<TransactionDetails> cartItems, int total, int paid, int change, string staffName, string receiptNo, string paymentMethod)
        {
            InitializeComponent();

            ReceiptTimeText.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ReceiptNoText.Text = $"レシートNo: {receiptNo}";
            StaffNameText.Text = $"担当: {staffName}";

  
            ReceiptItemsControl.ItemsSource = cartItems;

            TxtTotal.Text = $"¥{total:N0}";
            TxtPaid.Text = $"¥{paid:N0}";
            TxtChange.Text = $"¥{change:N0}";

        
            if (paymentMethod != "Cash")
            {
                PaymentMethodText.Text = $"お支払 ({paymentMethod})";
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}