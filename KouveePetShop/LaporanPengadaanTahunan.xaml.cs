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
    /// Interaction logic for LaporanPengadaanTahunan.xaml
    /// </summary>
    public partial class LaporanPengadaanTahunan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string tahun;
        DateTime tanggal = DateTime.Now;

        public LaporanPengadaanTahunan()
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT DATE_FORMAT(date, '%M') AS Bulan, SUM(total) AS JumlahPengeluaran FROM( SELECT pp.TANGGAL_PENGADAANPRODUK AS date, SUM(dp.TOTAL) as total from pengadaan_produk pp JOIN detail_pengadaan_produk dp ON pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK WHERE YEAR(pp.TANGGAL_PENGADAANPRODUK) = YEAR(Now()) GROUP BY pp.ID_TRANSAKSIPENGADAAN UNION SELECT '2020-01-01' AS date, 0 AS total UNION SELECT '2020-02-01' AS date, 0 AS total UNION SELECT '2020-03-01' AS date, 0 AS total UNION SELECT '2020-04-01' AS date, 0 AS total UNION SELECT '2020-05-01' AS date, 0 AS total UNION SELECT '2020-06-01' AS date, 0 AS total UNION SELECT '2020-07-01' AS date, 0 AS total UNION SELECT '2020-08-01' AS date, 0 AS total UNION SELECT '2020-09-01' AS date, 0 AS total UNION SELECT '2020-10-01' AS date, 0 AS total UNION SELECT '2020-11-01' AS date, 0 AS total UNION SELECT '2020-12-01' AS date, 0 AS total) AS merged GROUP BY DATE_FORMAT(date, '%Y%m') ORDER BY date ASC", conn);
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

        private void FillTextTotal()
        {
            // query nomor suplier
            string Query = "SELECT SUM(dp.TOTAL) AS Total FROM detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                   TotalPengadaanText.Text = (reader["Total"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            TanggalCetakText.Text = tanggal.ToString("dd MMMM yyyy");
            conn.Open();
            TampilDataGrid();
            FillTextTotal();
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
