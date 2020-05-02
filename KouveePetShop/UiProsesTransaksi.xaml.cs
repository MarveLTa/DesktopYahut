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
    /// Interaction logic for UiProsesTransaksi.xaml
    /// </summary>
    public partial class UiProsesTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        public UiProsesTransaksi()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        public void FillTextBox()
        {
            string Query = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            try
            {         
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    string nama = (reader["NAMA_CUSTOMER"].ToString());
                    //NamaCustomerText.RefreshCurrent();
                   // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();               
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT p.NAMA_PRODUK as nama, dt.JUMLAH as jumlah from detail_transaksi_produk dt JOIN produk p ON dt.ID_PRODUK = p.ID_PRODUK JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN WHERE h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "' ", conn);
            try
            {
                //conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                DataGrid.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
                conn.Close();
            }
        }

        private void HitungTotalDiskon()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(tr.DISKON) as DISKON from transaksi tr " +
                "JOIN customer cr ON tr.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE cr.NAMA_CUSTOMER LIKE '" + NamaCustomerText.Text + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    DiskonText.Text = (reader["DISKON"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void HitungTotalHarga()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.TOTAL) as TOTAL from detail_transaksi_produk dt " +
                "JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI " +
                "JOIN customer cr ON tr.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE cr.NAMA_CUSTOMER LIKE '" + NamaCustomerText.Text + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalHargaText.Text = (reader["TOTAL"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnBayar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE transaksi tr, customer cr SET tr.STATUS_PEMBAYARAN = 'Success' WHERE tr.ID_CUSTOMER = cr.ID_CUSTOMER AND cr.NAMA_CUSTOMER LIKE '" + NamaCustomerText.Text + "'";
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Berhasil dibayar!", "Success");
                    conn.Close();
                    this.Close();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
            
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();

            // query nama customer
            string Query = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            // query status customer
            string Query2 = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand statusCmd = new MySqlCommand(Query2, conn);
            MySqlDataReader reader2;
            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    NamaCustomerText.Text = (reader["NAMA_CUSTOMER"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();

                reader2 = statusCmd.ExecuteReader();

                while (reader2.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    StatusText.Text = (reader2["STATUS"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader2.Close();

                TampilDataGrid();
                HitungTotalDiskon();
                HitungTotalHarga();
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }
    }
}