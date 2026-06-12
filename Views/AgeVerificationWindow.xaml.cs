using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OICPOSレジ_2C29KS.Views
{
    /// <summary>
    /// AgeVerificationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AgeVerificationWindow : Window
    {

        public bool IsVerified { get; private set; } = false;

        public AgeVerificationWindow()
        {
            InitializeComponent();
        }

        // はい (Confirm) နှိပ်ရင် True ပေးပြီး ပိတ်မယ်
        private void YesBtn_Click(object sender, RoutedEventArgs e)
        {
            IsVerified = true;
            this.Close();
        }

        // いいえ (Cancel) နှိပ်ရင် False အတိုင်း ပိတ်မယ်
        private void NoBtn_Click(object sender, RoutedEventArgs e)
        {
            IsVerified = false;
            this.Close();
        }
    }
}

