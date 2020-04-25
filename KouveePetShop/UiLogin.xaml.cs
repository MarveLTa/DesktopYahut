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
using MySql.Data.MySqlClient;
using System.Data;

namespace KouveePetShop
{
    /// <summary>
    /// Interaction logic for UiLogin.xaml
    /// </summary>
    public partial class UiLogin : Window
    {       
        UiDashboard dashboardAdmin = new UiDashboard();
        public UiLogin()
        {
            InitializeComponent();          
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(UsernameText.Text == "" || PasswordText.SecurePassword.Length == 0)
                {
                    MessageBox.Show("Field tidak boleh kosong", "Warning");
                    return;
                }
                else
                {
                    string username = UsernameText.Text;
                    string password = PasswordText.Password;

                    MySqlConnection conn = new MySqlConnection("Server=localhost; User Id=root;Password=;Database=petshop");
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("Select * from pegawai where USERNAME = '" + username + "' and PASSWORD = '" + password + "'", conn);
                    cmd.CommandType = CommandType.Text;
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    adapter.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    if(ds.Tables[0].Rows.Count > 0)
                    {
                        string user = ds.Tables[0].Rows[0]["NAMA_PEGAWAI"].ToString() + " - " + ds.Tables[0].Rows[0]["ROLE"].ToString();
                        dashboardAdmin.RoleText.Text = user;

                        // User akan diarahkan berdasar pada role
                        string role = ds.Tables[0].Rows[0]["ROLE"].ToString();
                        switch (role)
                        {
                            case "Admin":
                                dashboardAdmin.Show();
                                this.Close();
                                break;
                            case "Kasir":
                                UiKasir dashboardKasir = new UiKasir();
                                dashboardKasir.Show();
                                this.Close();
                                break;
                            case "Customer Service":
                                UiCustomerService dashboardCS = new UiCustomerService();
                                dashboardCS.Show();
                                this.Close();
                                break;
                        } 
                        conn.Close();
                    }
                    else
                    {
                        MessageBox.Show("Masukkan username dan password yang valid!", "Warning");
                        conn.Close();
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                return;
            }
        }
    }
}
