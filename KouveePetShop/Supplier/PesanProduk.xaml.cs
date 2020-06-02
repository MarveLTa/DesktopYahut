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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Data;

namespace KouveePetShop
{
    /// <summary>
    /// Interaction logic for PesanProduk.xaml
    /// </summary>
    public partial class PesanProduk : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public PesanProduk()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                conn.Open();
                TampilDataGrid();
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Total()
        {
            try
            {
                using (MySqlCommand cmdMember = new MySqlCommand())
                {
                    cmdMember.Connection = conn;
                    cmdMember.CommandText = "UPDATE detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET dt.TOTAL = (SELECT sum(SUB_TOTAL - (SUB_TOTAL * 10/100)) FROM detail_transaksi_produk dt2 WHERE dt2.ID_PEMBAYARAN_PRODUK = dt.ID_PEMBAYARAN_PRODUK) WHERE cr.STATUS LIKE 'Member'";
                    cmdMember.ExecuteNonQuery();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT pp.NO_TRANSAKSIPENGADAAN, pp.ID_TRANSAKSIPENGADAAN, sp.NAMA_SUPPLIER, SUM(dp.JUMLAH * p.HARGA_PRODUK) as TOTAL, pp.STATUS_BARANG, pp.TANGGAL_PENGADAANPRODUK FROM pengadaan_produk pp JOIN detail_pengadaan_produk dp ON pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN JOIN supplier sp ON pp.ID_SUPPLIER = sp.ID_SUPPLIER JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK WHERE pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN AND pp.NO_TRANSAKSIPENGADAAN LIKE 'PO%' GROUP BY PP.NO_TRANSAKSIPENGADAAN ORDER BY PP.ID_TRANSAKSIPENGADAAN DESC", conn);
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
            }
        }

        private void CariPengadaanProdukText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari transaksi sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("SELECT pp.NO_TRANSAKSIPENGADAAN, pp.ID_TRANSAKSIPENGADAAN, sp.NAMA_SUPPLIER, SUM(dp.JUMLAH * p.HARGA_PRODUK) as TOTAL, pp.STATUS_BARANG, pp.TANGGAL_PENGADAANPRODUK FROM pengadaan_produk pp JOIN detail_pengadaan_produk dp ON pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN JOIN supplier sp ON pp.ID_SUPPLIER = sp.ID_SUPPLIER JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK WHERE pp.ID_TRANSAKSIPENGADAAN = dp.ID_TRANSAKSIPENGADAAN AND pp.NO_TRANSAKSIPENGADAAN LIKE 'PO%' AND sp.NAMA_SUPPLIER LIKE '" + CariPengadaanProdukText.Text + "%' GROUP BY PP.NO_TRANSAKSIPENGADAAN", conn);

                adp.Fill(dt);
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            TampilDataGrid();
            conn.Close();
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            TambahPengadaan TP = new TambahPengadaan();
            TP.Show();
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            TransaksiPengadaan TP = new TransaksiPengadaan();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if(selected_row != null)
            {
                TP.idTransaksi = selected_row["ID_TRANSAKSIPENGADAAN"].ToString();
                TP.NamaSupplierText.Text = selected_row["NAMA_SUPPLIER"].ToString();
                TP.TotalHargaText.Text = selected_row["TOTAL"].ToString();
                TP.Show();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditPengadaan EP = new EditPengadaan();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if(selected_row != null)
            {
                EP.idTransaksi = selected_row["ID_TRANSAKSIPENGADAAN"].ToString();
                EP.NamaSupplierText.Text = selected_row["NAMA_SUPPLIER"].ToString();
                EP.Show();
            }
        }    

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            string message = "Apakah anda ingin menghapus data ini ?";
            string caption = "Warning";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;

            try
            {
                MySqlCommand cmd;
                MySqlCommand cmd2;
                DataRowView row = (DataRowView)((Button)e.Source).DataContext;

                string query = "Delete from pengadaan_produk where ID_TRANSAKSIPENGADAAN = @1";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add(new MySqlParameter("@1", row["ID_TRANSAKSIPENGADAAN"]));

                conn.Open();

                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {
                    string queryDetail = "Delete from detail_pengadaan_produk where ID_TRANSAKSIPENGADAAN = @1";
                    cmd2 = new MySqlCommand(queryDetail, conn);
                    cmd2.Parameters.Add(new MySqlParameter("@1", row["ID_TRANSAKSIPENGADAAN"]));
                    cmd2.ExecuteNonQuery();

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Berhasil dihapus", "Success");
                    TampilDataGrid();
                    conn.Close();
                }
                else
                {
                    conn.Close();
                    return;
                }

            }
            catch (Exception err)
            {
                if (err is ConstraintException || err is MySqlException)
                {
                    MessageBox.Show("Data ini masih digunakan oleh tabel yang lain, silahkan pilih data yang lainnya!", "Warning");
                    conn.Close();
                    return;
                }
                else
                {
                    MessageBox.Show(err.Message);
                    conn.Close();
                }
            }
        }
    }
}
