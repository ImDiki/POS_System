using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using OICPOSレジ_2C29KS.Data;

namespace OICPOSレジ_2C29KS.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    string query = "SELECT * FROM ProductMaster";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ProductDataGrid.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("データ読み込みエラー: " + ex.Message);
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
       
            if (string.IsNullOrWhiteSpace(TxtCode.Text) || string.IsNullOrWhiteSpace(TxtName.Text) || string.IsNullOrWhiteSpace(TxtPrice.Text))
            {
                MessageBox.Show("全ての項目を入力してください。");
                return;
            }

        
            if (!int.TryParse(TxtPrice.Text, out int price))
            {
                MessageBox.Show("単価には数字を入力してください。");
                return;
            }

            try
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    string query = "INSERT INTO ProductMaster (ProductCode, ProductName, Price, Cost, Stock, IsAgeRestricted) " +
                             "VALUES (@code, @name, @price, @cost, @stock, @age)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@code", TxtCode.Text);
                        cmd.Parameters.AddWithValue("@name", TxtName.Text);
                        cmd.Parameters.AddWithValue("@price", price);
                        cmd.Parameters.AddWithValue("@cost", TxtCost.Text);        // Cost ကို 0 ထားသည်
                        cmd.Parameters.AddWithValue("@stock", TxtStock.Text);       // **ဒီနေရာမှာ Stock ကို 0 လို့ ထည့်လိုက်ပါ**
                        cmd.Parameters.AddWithValue("@age", false);     // AgeRestricted ကို false ထားသည်
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("保存しました");
                LoadProducts();
                TxtCode.Clear(); TxtName.Clear(); TxtPrice.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存エラー: " + ex.Message);
            }
        }
    }
}