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
    /// Interaction logic for LaporanPendapatanTahunan.xaml
    /// </summary>
    public partial class LaporanPendapatanTahunan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string tahun;
        DateTime tanggal = DateTime.Now;
        public LaporanPendapatanTahunan()
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
            MySqlCommand cmd = new MySqlCommand("SELECT DATE_FORMAT(date, '%M') AS Bulan, JasaLayanan, Produk, SUM(JasaLayanan + Produk) AS Total FROM (  SELECT tr.TANGGAL_TRANSAKSI AS date, SUM(dtJs.TOTAL) AS JasaLayanan, SUM(dtP.TOTAL) AS Produk from transaksi tr  LEFT JOIN detail_transaksi_produk dtP ON dtP.ID_TRANSAKSI = tr.ID_TRANSAKSI LEFT JOIN detail_transaksi_jasalayanan dtJs ON tr.ID_TRANSAKSI = dtJs.ID_TRANSAKSI WHERE YEAR(tr.TANGGAL_TRANSAKSI) = YEAR(Now()) AND tr.STATUS_PEMBAYARAN = 'Success' UNION SELECT '2020-01-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-02-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-03-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-04-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-05-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-06-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-07-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-08-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-09-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-10-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-11-01' AS date, 0 AS JasaLayanan, 0 AS Produk UNION SELECT '2020-12-01' AS date, 0 AS JasaLayanan, 0 AS Produk) AS merged GROUP BY DATE_FORMAT(date, '%Y%m')  ORDER BY date ASC", conn);
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

        private void HitungTotalHarga()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT SUM(TotalP + TotalJ) AS Total FROM(SELECT SUM(dt.TOTAL) AS TotalP, SUM(dt2.TOTAL) AS TotalJ FROM transaksi tr LEFT JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI LEFT JOIN detail_transaksi_jasalayanan dt2 ON tr.ID_TRANSAKSI = dt2.ID_TRANSAKSI) m", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalText.Text = (reader["Total"].ToString());
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
            TanggalCetakText.Text = tanggal.ToString("dd MMMM yyyy");
            conn.Open();
            TampilDataGrid();
            HitungTotalHarga();
            conn.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
