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
    /// Interaction logic for TransaksiProduk.xaml
    /// </summary>
    public partial class TransaksiProduk : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public TransaksiProduk()
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select tr.NO_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, dt.TOTAL, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, tr.ID_PEGAWAI2 from transaksi tr JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI " +
                "JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN where tr.NO_TRANSAKSI LIKE 'PR%'", conn);
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

        private void Diskon()
        {
            try
            {
                using(MySqlCommand cmd = new MySqlCommand())
                {
                    conn.Open();
                    cmd.Connection = conn;
                    cmd.CommandText = "Update detail_transaksi_produk dt set SUB_TOTAL = (dt.DISKON * (dt.JUMLAH * p.HARGA_PRODUK) / 100) join produk p where dt.ID_PRODUK = p.ID_PRODUK ";
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CariTransaksiProdukText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
