using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using OICPOSレジ_2C29KS.Data;

namespace OICPOSレジ_2C29KS.Views
{
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                using (SqlConnection con = DatabaseHelper.GetConnection())
                {
                    con.Open();
                    
                    string query = "SELECT * FROM Transactions";
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Debug အတွက်: အကယ်၍ dt ထဲမှာ row မရှိရင် MessageBox နဲ့ ပြောပေးမယ်
                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("販売履歴はありません");
                    }
                    else
                    {
                        HistoryDataGrid.ItemsSource = dt.DefaultView;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}