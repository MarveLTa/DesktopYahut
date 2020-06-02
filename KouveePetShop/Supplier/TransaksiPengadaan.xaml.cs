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
    public partial class TransaksiPengadaan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string status;
        DateTime tanggal = DateTime.Now;
        

        public TransaksiPengadaan()
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
            MySqlCommand cmd = new MySqlCommand("SELECT ID_DETAIL_PENGADAAN, NAMA_PRODUK, JUMLAH FROM detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN WHERE dp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'", conn);
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

        private void CekStatus()
        {
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT STATUS_BARANG as STATUS from pengadaan_produk WHERE ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'", conn);
                MySqlDataReader reader;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string statusData = (reader["STATUS"].ToString());
                    status = statusData;
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void BtnBayar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CekStatus();
                conn.Open();

                if (status == "Success" || status == "On Process")
                {
                    MessageBox.Show("Transaksi sudah dibayar!", "Warning");
                    conn.Close();
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        string tanggalUpdate = tanggal.ToString("yyyy-MM-dd");
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE pengadaan_produk pp, detail_pengadaan_produk dp SET pp.STATUS_BARANG = 'On Process', pp.TANGGAL_PENGADAANPRODUK = @tanggalPengadaan, dp.TANGGAL_CETAK = @tanggalCetak WHERE pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
                        cmd.CommandType = CommandType.Text;

                        cmd.Parameters.AddWithValue("@tanggalPengadaan", tanggalUpdate);
                        cmd.Parameters.AddWithValue("@tanggalCetak", tanggalUpdate);
                        cmd.ExecuteNonQuery();

                        

                       // UpdateStok();
                        MessageBox.Show("Berhasil dibayar!", "Success");
                        conn.Close();

                        StrukPengadaaan SP = new StrukPengadaaan();
                        SP.idTransaksi = idTransaksi;
                        SP.Show();
                        this.Close();
                    }
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
            TampilDataGrid();
            conn.Close();
        }

        private void BtnKonfirmasi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CekStatus();
                conn.Open();

                if(status == "Success")
                {
                    MessageBox.Show("Barang sudah di konfirmasi!", "Warning");
                    conn.Close();
                    return;
                }
                else if(status == null)
                {
                    MessageBox.Show("Bayar barang terlebih dahulu!", "Warning");
                    conn.Close();
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "UPDATE pengadaan_produk pp, detail_pengadaan_produk dp SET pp.STATUS_BARANG = 'Success' WHERE pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
                        cmd.ExecuteNonQuery();

                        // UpdateStok();
                        MessageBox.Show("Barang berhasil di konfirmasi!", "Success");
                        conn.Close();
                        this.Close();
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
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
    }
}
