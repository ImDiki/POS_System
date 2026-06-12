using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using OICPOSレジ_2C29KS.Data;
using OICPOSレジ_2C29KS.Views;

namespace OICPOSレジ_2C29KS
{
    public partial class MainWindow : Window
    {
        // လက်ရှိ Login ဝင်ထားသော ဝန်ထမ်း အချက်အလက်များ
        public static int CurrentStaffID = 0;
        public static string CurrentStaffName = "";
        public static string CurrentStaffRole = "";
        public static bool IsLoggedIn = false;

        public MainWindow()
        {
            InitializeComponent();
            StartClock();
        }

        // --- အချိန်ကို Real-time ပြသရန် ---
        private void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => {
                DateTimeText.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            };
            timer.Start();
        }

        // --- Sidebar အဖွင့်/အပိတ် ခလုတ်များ ---
        private void CloseSidebar_Click(object sender, RoutedEventArgs e)
        {
            SidebarPanel.Visibility = Visibility.Collapsed;
            OpenSidebarBtn.Visibility = Visibility.Visible;
        }

        private void OpenSidebar_Click(object sender, RoutedEventArgs e)
        {
            SidebarPanel.Visibility = Visibility.Visible;
            OpenSidebarBtn.Visibility = Visibility.Collapsed;
        }

        // --- ログイン (Login / Logout) ခလုတ် နှိပ်သောအခါ ---
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IsLoggedIn)
            {
                // Logout လုပ်ခြင်း
                IsLoggedIn = false;
                CurrentStaffID = 0;
                CurrentStaffName = "";
                CurrentStaffRole = "";

                UserText.Text = "担当：未ログイン";
                LoginBtn.Content = "ログイン";
                LoginBtn.Background = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#2C3E50")); // မူလအရောင်

                // View အကုန်ပိတ်ပြီး Welcome စာသားပဲ ပြန်ပြမယ်
                MainContentArea.Content = null;
                WelcomeText.Visibility = Visibility.Visible;

                MessageBox.Show("ログアウトしました。(Logout အောင်မြင်ပါသည်)", "Logout", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Login Box ကို ဖွင့်ပေးခြင်း
                LoginViewPanel.Visibility = Visibility.Visible;
                StaffIdTextBox.Clear();
                StaffIdTextBox.Focus();
            }
        }

        // --- Staff ID ရိုက်ပြီး Enter ခေါက်သောအခါ (Database ဖြင့် စစ်ဆေးခြင်း) ---
        private void StaffIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string inputId = StaffIdTextBox.Text.Trim();
                if (string.IsNullOrEmpty(inputId)) return;

                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    try
                    {
                        con.Open();
                        string query = "SELECT StaffName, Role FROM StaffInfo WHERE StaffID = @id";
                        using (SqlCommand cmd = new SqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@id", inputId);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read()) // ID မှန်ကန်ပါက
                                {
                                    IsLoggedIn = true;
                                    CurrentStaffID = Convert.ToInt32(inputId);
                                    CurrentStaffName = reader["StaffName"].ToString();
                                    CurrentStaffRole = reader["Role"].ToString();

                                    // UI တွင် ဝန်ထမ်းအမည်ပြောင်းပေးခြင်း
                                    UserText.Text = $"担当：{CurrentStaffName} ({CurrentStaffRole})";
                                    LoginBtn.Content = "ログアウト (Logout)";
                                    LoginBtn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);

                                    // Login Box ကို ပြန်ပိတ်မယ်
                                    LoginViewPanel.Visibility = Visibility.Collapsed;

                                    MessageBox.Show($"{CurrentStaffName} さん、お疲れ様です！", "Login Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {
                                    MessageBox.Show("社員番号が間違っています。(Staff ID မှားယွင်းနေပါသည်)", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                    StaffIdTextBox.Clear();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Database Error: " + ex.Message);
                    }
                }
            }
        }

        // --- 会計 (Checkout) ခလုတ် ---
        private void CheckoutBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn)
            {
                MessageBox.Show("先にログインしてください.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Welcome စာသားကို ဖျောက်ပြီး CheckoutView ကို လှမ်းခေါ်မယ်
            WelcomeText.Visibility = Visibility.Collapsed;
            MainContentArea.Content = new CheckoutView();
        }

        // --- 履歴 (History) ခလုတ် ---
        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            WelcomeText.Visibility = Visibility.Collapsed;
            MainContentArea.Content = new HistoryView();
        }

        private void ProductBtn_Click(object sender, RoutedEventArgs e)
        {
            WelcomeText.Visibility = Visibility.Collapsed;
            MainContentArea.Content = new ProductView();
        }
    }
}