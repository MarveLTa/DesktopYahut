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
    /// Interaction logic for StrukJasaLayanan.xaml
    /// </summary>
    public partial class StrukJasaLayanan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;

        public StrukJasaLayanan()
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT js.NAMA_JASA_LAYANAN as nama, js.HARGA_JASA_LAYANAN as hargaProduk, dt.JUMLAH as jumlah, dt.SUB_TOTAL as totalHarga from detail_transaksi_jasalayanan dt JOIN jasa_layanan js ON dt.ID_JASA_LAYANAN = js.ID_JASA_LAYANAN JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "' ", conn);
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

        private void FillTextBoxMember()
        {
            // query nama customer
            string Query = "SELECT cr.NAMA_CUSTOMER FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "JOIN transaksi tr ON h.ID_HEWAN = tr.ID_HEWAN " +
                "where tr.ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            // query nama hewan
            string Query2 = "SELECT h.NAMA_HEWAN FROM hewan h JOIN transaksi tr ON h.ID_HEWAN = tr.ID_HEWAN" +
                " where tr.ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd2 = new MySqlCommand(Query2, conn);
            MySqlDataReader reader2;

            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    NamaCustomerText.Text = (reader["NAMA_CUSTOMER"].ToString());
                    //NamaCustomerText.RefreshCurrent();
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();

                reader2 = namaCmd2.ExecuteReader();

                while (reader2.Read())
                {
                    NamaHewanText.Text = (reader2["NAMA_HEWAN"].ToString());
                }

                reader2.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxPegawai()
        {
            // query nama pegawai
            string Query = "SELECT pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2 FROM transaksi tr JOIN pegawai pg1 ON tr.ID_PEGAWAI = pg1.ID_PEGAWAI JOIN pegawai pg2 ON tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI where tr.ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    CustomerServiceText.Text = (reader["nama1"].ToString());
                    KasirText.Text = (reader["nama2"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxTelp()
        {
            // query nomor customer
            string Query = "SELECT cr.NOTELP_CUSTOMER as NOMOR FROM transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER where tr.ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    TelpText.Text = (reader["NOMOR"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxNoTransaksi()
        {
            // query nomor transaksi
            string Query = "SELECT NO_TRANSAKSI FROM transaksi where ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    NomorTransaksiText.Text = (reader["NO_TRANSAKSI"].ToString());
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void FillTextBoxTanggal()
        {
            // query tanggal transaksi
            string Query = "SELECT TANGGAL_TRANSAKSI FROM transaksi where ID_TRANSAKSI = '" + idTransaksi + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;


            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    TanggalTransaksiText.Text = Convert.ToDateTime(reader["TANGGAL_TRANSAKSI"]).ToString("dd MMMM yyyy");
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void HitungSubTotalHarga()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.SUB_TOTAL) as SUB_TOTAL from detail_transaksi_jasalayanan dt " +
                "JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI " +
                "JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN " +
                "JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    SubTotalText.Text = (reader["SUB_TOTAL"].ToString());
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

        private void HitungTotalDiskon()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(tr.DISKON) as DISKON from transaksi tr " +
                "JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN " +
                "JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
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
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void HitungTotalHarga()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.TOTAL) as TOTAL from detail_transaksi_jasalayanan dt " +
                "JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI " +
                "JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN " +
                "JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalText.Text = (reader["TOTAL"].ToString());
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
            conn.Open();
            FillTextBoxMember();
            FillTextBoxPegawai();
            FillTextBoxTelp();
            FillTextBoxNoTransaksi();
            FillTextBoxTanggal();
            HitungSubTotalHarga();
            HitungTotalDiskon();
            HitungTotalHarga();
            TampilDataGrid();
            conn.Close();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }
    }
}
