﻿using System;
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
    /// Interaction logic for LaporanJasaLayananTerlaris.xaml
    /// </summary>
    public partial class LaporanJasaLayananTerlaris : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string tahun;
        DateTime tanggal = DateTime.Now;
        public LaporanJasaLayananTerlaris()
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT DATE_FORMAT(date, '%M') AS Bulan, NamaLayanan, MAX(total) AS JumlahPenjualan FROM ( SELECT tr.TANGGAL_TRANSAKSI AS date, js.NAMA_JASA_LAYANAN AS NamaLayanan, SUM(dt.JUMLAH) as total from transaksi tr JOIN detail_transaksi_jasalayanan dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI JOIN jasa_layanan js ON dt.ID_JASA_LAYANAN = js.ID_JASA_LAYANAN WHERE YEAR(tr.TANGGAL_TRANSAKSI) = YEAR(Now()) AND tr.STATUS_PEMBAYARAN = 'Success' GROUP BY tr.ID_TRANSAKSI, js.ID_JASA_LAYANAN UNION SELECT '2020-01-01' AS date, '-' AS NamaLayanan, 0 AS total UNION SELECT '2020-02-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-03-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-04-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-05-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-06-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-07-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-08-01' AS date, '-' AS NamaLayanan, 0 AS total UNION SELECT '2020-09-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-10-01' AS date, '-' AS NamaLayanan, 0 AS total UNION SELECT '2020-11-01' AS date, '-' AS NamaLayanan, 0 AS total UNION  SELECT '2020-12-01' AS date, '-' AS NamaLayanan, 0 AS total) AS merged GROUP BY DATE_FORMAT(date, '%Y%m') ORDER BY date ASC", conn);
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
            conn.Close();
        }
    }
}
