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
    /// Interaction logic for LaporanPendapatanBulanan.xaml
    /// </summary>
    public partial class LaporanPendapatanBulanan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string tahun;
        public string bulan;
        DateTime tanggal = DateTime.Now;
        public LaporanPendapatanBulanan()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);

                Loaded += Window_Loaded;
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TampilDataGridLayanan()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT JasaLayanan, SUM(harga) AS Harga FROM (SELECT tr.TANGGAL_TRANSAKSI AS date, js.NAMA_JASA_LAYANAN AS JasaLayanan, SUM(dt.TOTAL) AS harga from transaksi tr  JOIN detail_transaksi_jasalayanan dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI JOIN jasa_layanan js ON dt.ID_JASA_LAYANAN = js.ID_JASA_LAYANAN WHERE MONTH(tr.TANGGAL_TRANSAKSI) = 5 AND YEAR(tr.TANGGAL_TRANSAKSI) = YEAR(NOW()) AND tr.STATUS_PEMBAYARAN = 'Success' GROUP BY tr.ID_TRANSAKSI) AS m GROUP BY JasaLayanan", conn);
            try
            {
                //conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                DataGridLayanan.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
                conn.Close();
            }
        }

        private void TampilDataGridProduk()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT Produk, SUM(harga) AS Harga FROM ( SELECT tr.TANGGAL_TRANSAKSI AS date, p.NAMA_PRODUK AS Produk, SUM(dt.TOTAL) AS harga from transaksi tr  JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI JOIN produk p ON dt.ID_PRODUK = p.ID_PRODUK WHERE MONTH(tr.TANGGAL_TRANSAKSI) = 5 AND YEAR(tr.TANGGAL_TRANSAKSI) = YEAR(NOW()) AND tr.STATUS_PEMBAYARAN = 'Success' AND tr.ID_TRANSAKSI = dt.ID_TRANSAKSI GROUP BY tr.ID_TRANSAKSI, p.NAMA_PRODUK) AS m GROUP BY Produk", conn);
            try
            {
                //conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                DataGridProduk.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
                conn.Close();
            }
        }

        private void FillTotalLayanan()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.TOTAL) as TOTAL from detail_transaksi_jasalayanan dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER WHERE tr.ID_TRANSAKSI = dt.ID_TRANSAKSI AND tr.STATUS_PEMBAYARAN = 'Success' AND MONTH(tr.TANGGAL_TRANSAKSI) = 5", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalLayanan.Text = (reader["TOTAL"].ToString());
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

        private void FillTotalProduk()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.TOTAL) as TOTAL from detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER WHERE tr.ID_TRANSAKSI = dt.ID_TRANSAKSI AND tr.STATUS_PEMBAYARAN = 'Success' AND MONTH(tr.TANGGAL_TRANSAKSI) = 5", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalProduk.Text = (reader["TOTAL"].ToString());
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


        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TanggalCetakText.Text = tanggal.ToString("dd MMMM yyyy");
            conn.Open();
            TampilDataGridLayanan();
            TampilDataGridProduk();
            FillTotalLayanan();
            FillTotalProduk();
            conn.Close();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                printDialog.PageRangeSelection = PageRangeSelection.AllPages;
                printDialog.UserPageRangeEnabled = true;
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "invoice");
                }
                this.Close();
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}
