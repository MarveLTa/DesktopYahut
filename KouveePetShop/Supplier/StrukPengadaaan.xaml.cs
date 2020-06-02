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
    /// Interaction logic for StrukPengadaaan.xaml
    /// </summary>
    public partial class StrukPengadaaan : Window
    {

        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;

        public StrukPengadaaan()
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT NAMA_PRODUK, SATUAN, JUMLAH FROM detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN WHERE dp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'", conn);
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

        private void FillTextBoxNamaSupplier()
        {
            // query nama suplier
            string Query = "SELECT sp.NAMA_SUPPLIER as NAMA FROM pengadaan_produk pp JOIN supplier sp ON pp.ID_SUPPLIER = sp.ID_SUPPLIER where pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    NamaSupplierText.Text = (reader["NAMA"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxNomorSupplier()
        {
            // query nomor suplier
            string Query = "SELECT sp.NOTELP_SUPPLIER as NOMOR FROM pengadaan_produk pp JOIN supplier sp ON pp.ID_SUPPLIER = sp.ID_SUPPLIER where pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    NomorSupplierText.Text = (reader["NOMOR"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxNomorTransaksi()
        {
            // query nomor suplier
            string Query = "SELECT pp.NO_TRANSAKSIPENGADAAN as NOMOR FROM pengadaan_produk pp JOIN detail_pengadaan_produk dp ON pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN where pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    NomorTransaksiText.Text = (reader["NOMOR"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxTanggalTransaksi()
        {
            // query tanggal transaksi
            string Query = "SELECT TANGGAL_PENGADAANPRODUK FROM pengadaan_produk where ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    TanggalTransaksiText.Text = Convert.ToDateTime(reader["TANGGAL_PENGADAANPRODUK"]).ToString("dd MMMM yyyy");
                    //TanggalTransaksiText.Text = (reader["TANGGAL_TRANSAKSI"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxTanggalCetak()
        {
            // query tanggal cetak
            string Query = "SELECT TANGGAL_CETAK FROM detail_pengadaan_produk where ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    TanggalCetakText.Text = Convert.ToDateTime(reader["TANGGAL_CETAK"]).ToString("dd MMMM yyyy");
                    //TanggalTransaksiText.Text = (reader["TANGGAL_TRANSAKSI"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            FillTextBoxNamaSupplier();
            FillTextBoxNomorSupplier();
            FillTextBoxNomorTransaksi();
            FillTextBoxTanggalCetak();
            FillTextBoxTanggalTransaksi();
            TampilDataGrid();
            conn.Close();
        }
    }
}
