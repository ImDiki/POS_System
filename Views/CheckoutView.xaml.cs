using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using OICPOSレジ_2C29KS.Data;
using OICPOSレジ_2C29KS.Models;
using OICPOSレジ_2C29KS.Views;

namespace OICPOSレジ_2C29KS
{
    public partial class CheckoutView : UserControl
    {
        private int count10000 = 5; private int count5000 = 10; private int count1000 = 20;
        private int count500 = 30; private int count100 = 50; private int count50 = 50; private int count10 = 100;
        private int count5 = 50; private int count1 = 100;

        private int todaySales = 0;
        private int todayProfit = 0;

        private ObservableCollection<TransactionDetails> cartList = new ObservableCollection<TransactionDetails>();
        private ObservableCollection<TransactionDetails> lastCartList = new ObservableCollection<TransactionDetails>();

        private int finalTotalAmount = 0;
        private int finalTotalCost = 0;

        private string lastReceiptNo = "";
        private string lastPaymentMethod = "";
        private int lastPaid = 0;
        private int lastChange = 0;
        private int lastTotalAmount = 0;

        public CheckoutView()
        {
            InitializeComponent();
            UpdateStatusUI();
            CartDataGrid.ItemsSource = cartList;

            Loaded += (s, e) => {
                HookAllButtons(this);
                BarcodeTextBox.Focus();
            };
        }

        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputCode = BarcodeTextBox.Text.Trim();
                if (string.IsNullOrEmpty(inputCode)) return;

                FetchProductFromDatabase(inputCode);
                BarcodeTextBox.Clear();
                BarcodeTextBox.Focus();
            }
        }

        private void FetchProductFromDatabase(string barcode)
        {
            using (SqlConnection con = DatabaseHelper.GetConnection())
            {
                try
                {
                    con.Open();
                    string query = "SELECT ProductName, Price, Cost, IsAgeRestricted FROM ProductMaster WHERE ProductCode = @code";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@code", barcode);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader["ProductName"].ToString();
                                int price = Convert.ToInt32(reader["Price"]);
                                int cost = Convert.ToInt32(reader["Cost"]);
                                bool isAgeRestricted = Convert.ToBoolean(reader["IsAgeRestricted"]);

                                if (isAgeRestricted)
                                {
                                    AgeVerificationWindow ageWindow = new AgeVerificationWindow();
                                    ageWindow.ShowDialog();
                                    if (!ageWindow.IsVerified)
                                    {
                                        MessageBox.Show("販売できません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Stop);
                                        return;
                                    }
                                }

                                AddToCart(barcode, name, price, cost);
                            }
                            else
                            {
                                MessageBox.Show("該当する商品がありません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("エラー: " + ex.Message);
                }
            }
        }

        private void AddToCart(string code, string name, int price, int cost)
        {
            TransactionDetails existingItem = null;
            foreach (var item in cartList)
            {
                if (item.ProductCode == code) { existingItem = item; break; }
            }

            if (existingItem != null)
            {
                existingItem.Quantity += 1;
                existingItem.Subtotal = existingItem.Price * existingItem.Quantity;
                finalTotalCost += cost;
                CartDataGrid.Items.Refresh();
            }
            else
            {
                cartList.Add(new TransactionDetails { ProductCode = code, ProductName = name, Price = price, Quantity = 1, Subtotal = price });
                finalTotalCost += cost;
            }
            UpdateTotalAmount();
        }

        private void UpdateTotalAmount()
        {
            finalTotalAmount = 0;
            foreach (var item in cartList) { finalTotalAmount += item.Subtotal; }
            TotalAmountText.Text = $"¥{finalTotalAmount:N0}";
            CalculateLiveChange();
        }

        private void CalculateLiveChange()
        {
            int paid = 0;
            int.TryParse(CashInputBox.Text, out paid);
            ChangeText.Text = (paid >= finalTotalAmount && finalTotalAmount > 0) ? $"¥{paid - finalTotalAmount:N0}" : "¥0";
        }

        private void HookAllButtons(DependencyObject parent)
        {
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is Button btn)
                {
                    if (btn.Name != "PayBtn" && btn.Name != "BtnPayPay" && btn.Name != "BtnICOCA" && btn.Name != "BtnCredit" &&
                        btn.Name != "UnlockStatusBtn" && btn.Name != "PrintReceiptBtn" &&
                        btn.Content?.ToString() != "キャンセル" && btn.Content?.ToString() != "隠す" &&
                        btn.Content?.ToString() != "両替" && btn.Content?.ToString() != "補充" && btn.Content?.ToString() != "-")
                    {
                        btn.Click += NumpadAndQuickCash_Click;
                    }
                }
                else HookAllButtons(child);
            }
        }

        private void NumpadAndQuickCash_Click(object sender, RoutedEventArgs e)
        {
            string content = ((Button)sender).Content.ToString();
            int currentInput = 0;
            int.TryParse(CashInputBox.Text, out currentInput);

            if (content == "C") CashInputBox.Text = "0";
            else if (content == "¥10,000") CashInputBox.Text = (currentInput + 10000).ToString();
            else if (content == "¥5,000") CashInputBox.Text = (currentInput + 5000).ToString();
            else if (content == "¥1,000") CashInputBox.Text = (currentInput + 1000).ToString();
            else if (content == "00") { if (CashInputBox.Text != "0") CashInputBox.Text += "00"; }
            else CashInputBox.Text = (CashInputBox.Text == "0") ? content : CashInputBox.Text + content;

            CalculateLiveChange();
        }

        private void PayBtn_Click(object sender, RoutedEventArgs e) { ProcessPayment("現金"); }
        private void CashlessBtn_Click(object sender, RoutedEventArgs e) { ProcessPayment(((Button)sender).Content.ToString()); }

        private void ProcessPayment(string paymentMethod)
        {
            // ၁။ ငွေပမာဏ စစ်ဆေးခြင်း
            if (finalTotalAmount == 0) return;
            int paid = 0, change = 0;

            if (paymentMethod == "現金")
            {
                if (!int.TryParse(CashInputBox.Text, out paid)) { MessageBox.Show("金額を正しく入力してください。"); return; }
                if (paid < finalTotalAmount) { MessageBox.Show("金額が足りません。"); return; }
                change = paid - finalTotalAmount;

                // Drawer အတွက် ပိုက်ဆံတွက်ချက်မှု (မူလ Logic)
                int amt = paid;
                count10000 += amt / 10000; amt %= 10000;
                count5000 += amt / 5000; amt %= 5000;
                count1000 += amt / 1000; amt %= 1000;
                count500 += amt / 500; amt %= 500;
                count100 += amt / 100; amt %= 100;
                count50 += amt / 50; amt %= 50;
                count10 += amt / 10; amt %= 10;
                count5 += amt / 5; amt %= 5;
                count1 += amt;

                // Change အတွက် နုတ်ခြင်း
                int tempChange = change;
                count10000 -= tempChange / 10000; tempChange %= 10000;
                count5000 -= tempChange / 5000; tempChange %= 5000;
                count1000 -= tempChange / 1000; tempChange %= 1000;
                count500 -= tempChange / 500; tempChange %= 500;
                count100 -= tempChange / 100; tempChange %= 100;
                count50 -= tempChange / 50; tempChange %= 50;
                count10 -= tempChange / 10; tempChange %= 10;
                count5 -= tempChange / 5; tempChange %= 5;
                count1 -= tempChange;
            }
            else { paid = finalTotalAmount; change = 0; }

            // ၂။ Database ထဲသို့ အရောင်းမှတ်တမ်းသိမ်းခြင်း
            try
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    // ReceiptNo ကို identity အဖြစ်ထားထားလို့ ဖြုတ်ထားပါသည်
                    string insertQuery = "INSERT INTO Transactions (DateTime, TotalAmount, CashPaid, CashChange, StaffID) " +
                                         "VALUES (@d, @t, @cp, @cc, @s)";
                    using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@d", DateTime.Now);
                        cmd.Parameters.AddWithValue("@t", finalTotalAmount);
                        cmd.Parameters.AddWithValue("@cp", paid);
                        cmd.Parameters.AddWithValue("@cc", change);
                        cmd.Parameters.AddWithValue("@s", MainWindow.CurrentStaffID);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("DB Error: " + ex.Message); return; }

            // ၃။ ReceiptWindow ကို ခေါ်ပြခြင်း
            string staffName = string.IsNullOrEmpty(MainWindow.CurrentStaffName) ? "ゲスト" : MainWindow.CurrentStaffName;
            string receiptNo = DateTime.Now.ToString("yyMMddHHmm");

            ReceiptWindow receiptWin = new ReceiptWindow(
                new ObservableCollection<TransactionDetails>(cartList),
                finalTotalAmount,
                paid,
                change,
                staffName,
                receiptNo,
                paymentMethod
            );

            receiptWin.ShowDialog();

            // ၄။ UI များ Reset လုပ်ခြင်း
            cartList.Clear();
            finalTotalCost = 0;
            finalTotalAmount = 0;
            CartDataGrid.Items.Refresh();
            CashInputBox.Text = "0";
            ChangeText.Text = "¥0";
            TotalAmountText.Text = "¥0";
            UpdateStatusUI();
            BarcodeTextBox.Focus();
        }

        private void PrintReceiptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (lastCartList.Count == 0)
            {
                MessageBox.Show("データがありません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string staffName = string.IsNullOrEmpty(MainWindow.CurrentStaffName) ? "ゲスト" : MainWindow.CurrentStaffName;
            SaveReceiptAsText(lastReceiptNo, staffName, lastPaymentMethod, lastPaid, lastChange, lastCartList, lastTotalAmount);

            MessageBox.Show("領収書を出力しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
            BarcodeTextBox.Focus();
        }

        private void SaveReceiptAsText(string receiptNo, string staffName, string paymentMethod, int paid, int change, ObservableCollection<TransactionDetails> items, int total)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string folderPath = System.IO.Path.Combine(desktopPath, "POS_Receipts");
                if (!System.IO.Directory.Exists(folderPath)) System.IO.Directory.CreateDirectory(folderPath);

                string filePath = System.IO.Path.Combine(folderPath, $"Receipt_{receiptNo}.txt");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("====================================");
                    writer.WriteLine("       OIC CONVENIENCE STORE        ");
                    writer.WriteLine("    大阪情報コンピュータ専門学校    ");
                    writer.WriteLine("====================================");
                    writer.WriteLine($"レシートNo: {receiptNo}");
                    writer.WriteLine($"日時: {DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}");
                    writer.WriteLine($"担当: {staffName}");
                    writer.WriteLine("------------------------------------");
                    foreach (var item in items) { writer.WriteLine($"{item.ProductName} x{item.Quantity} ... ¥{item.Subtotal:N0}"); }
                    writer.WriteLine("------------------------------------");
                    writer.WriteLine($"合計: ¥{total:N0}");
                    if (paymentMethod == "現金")
                    {
                        writer.WriteLine($"お預かり: ¥{paid:N0}");
                        writer.WriteLine($"お釣り: ¥{change:N0}");
                    }
                    else { writer.WriteLine($"お支払 ({paymentMethod}): ¥{total:N0}"); }
                    writer.WriteLine("====================================");
                    writer.WriteLine(" ありがとうございました！ ");
                    writer.WriteLine("====================================");
                }
            }
            catch { MessageBox.Show("保存に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void UpdateStatusUI()
        {
            Bill10000Text.Text = $"¥10000 ({count10000}枚)";
            Bill5000Text.Text = $"¥5000 ({count5000}枚)";
            Bill1000Text.Text = $"¥1000 ({count1000}枚)";
            Coin500Text.Text = $"¥500 ({count500}枚)";
            Coin100Text.Text = $"¥100 ({count100}枚)";
            Coin50Text.Text = $"¥50 ({count50}枚)";
            Coin10Text.Text = $"¥10 ({count10}枚)";
            Coin5Text.Text = $"¥5 ({count5}枚)";
            Coin1Text.Text = $"¥1 ({count1}枚)";

            int totalDrawerCash = (count10000 * 10000) + (count5000 * 5000) + (count1000 * 1000) +
                                  (count500 * 500) + (count100 * 100) + (count50 * 50) +
                                  (count10 * 10) + (count5 * 5) + (count1 * 1);

            if (DrawerTotalText != null)
            {
                DrawerTotalText.Text = $"ドロア合計: ¥{totalDrawerCash:N0}";
            }
        }

        private void UnlockStatusBtn_Click(object sender, RoutedEventArgs e)
        {
            SecurityPopupPanel.Visibility = Visibility.Visible;
            SecurityIdBox.Clear(); SecurityIdBox.Focus();
        }

        private void CancelSecurity_Click(object sender, RoutedEventArgs e)
        {
            SecurityPopupPanel.Visibility = Visibility.Collapsed;
            BarcodeTextBox.Focus();
        }

        private void SecurityIdBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (SecurityIdBox.Text.Trim() == MainWindow.CurrentStaffID.ToString())
                {
                    SecurityPopupPanel.Visibility = Visibility.Collapsed;
                    DefaultStatusPanel.Visibility = Visibility.Collapsed;
                    StatusDataPanel.Visibility = Visibility.Visible;
                    BarcodeTextBox.Focus();
                }
                else
                {
                    MessageBox.Show("番号が間違っています。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    SecurityIdBox.Clear();
                }
            }
        }

        private void HideStatusBtn_Click(object sender, RoutedEventArgs e)
        {
            StatusDataPanel.Visibility = Visibility.Collapsed;
            DefaultStatusPanel.Visibility = Visibility.Visible;
            BarcodeTextBox.Focus();
        }

        private void BtnExchange_Click(object sender, RoutedEventArgs e)
        {
            int type = int.Parse(((Button)sender).Tag.ToString());
            bool success = false;

            switch (type)
            {
                case 10000: if (count10000 >= 1) { count10000--; count5000 += 2; success = true; } break;
                case 5000: if (count5000 >= 1) { count5000--; count1000 += 5; success = true; } break;
                case 1000: if (count1000 >= 1) { count1000--; count500 += 2; success = true; } break;
                case 500: if (count500 >= 1) { count500--; count100 += 5; success = true; } break;
                case 100: if (count100 >= 1) { count100--; count50 += 2; success = true; } break;
                case 50: if (count50 >= 1) { count50--; count10 += 5; success = true; } break;
                case 10: if (count10 >= 1) { count10--; count5 += 2; success = true; } break;
                case 5: if (count5 >= 1) { count5--; count1 += 5; success = true; } break;
            }

            if (success) { UpdateStatusUI(); }
            else { MessageBox.Show("不足しています。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning); }
        }

        private void BtnFill_Click(object sender, RoutedEventArgs e)
        {
            int type = int.Parse(((Button)sender).Tag.ToString());

            switch (type)
            {
                case 10000: count10000 += 10; break;
                case 5000: count5000 += 10; break;
                case 1000: count1000 += 20; break;
                case 500: count500 += 50; break;
                case 100: count100 += 50; break;
                case 50: count50 += 50; break;
                case 10: count10 += 50; break;
                case 5: count5 += 50; break;
                case 1: count1 += 50; break;
            }
            UpdateStatusUI();
        }
    }
}