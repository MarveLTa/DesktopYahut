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
    /// Interaction logic for EditProdukPengadaan.xaml
    /// </summary>
    public partial class EditProdukPengadaan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string idDetail;
        public EditProdukPengadaan()
        {
            InitializeComponent();

            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);
            conn.Open();
            FillComboBoxNamaProduk();
            conn.Close();
        }

        public void FillComboBoxNamaProduk()
        {
            // Ambil ID dan Nama produk dari tabel produk ke combobox
            string Query = "select ID_PRODUK, NAMA_PRODUK from petshop.produk;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idProduk = reader.GetInt32("ID_PRODUK");
                    string namaProduk = reader.GetString("NAMA_PRODUK");
                    ComboBoxNamaProduk.Items.Add(idProduk + " - " + namaProduk);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBoxNamaProduk.Text))
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                return;
            }
            else
            {
                conn.Open();
                adapter = new MySqlDataAdapter("update detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN SET dp.ID_PRODUK = '" + ComboBoxNamaProduk.SelectedValue + "', dp.JUMLAH = '" + JumlahProdukText.Text + "' WHERE dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN AND pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "' AND dp.ID_DETAIL_PENGADAAN = '" + idDetail + "'", conn);
                adapter.Fill(ds, "detail_pengadaan_produk");
                MessageBox.Show("Edit berhasil!", "Success");
                conn.Close();

                using (MySqlCommand cmdUpdateNama = new MySqlCommand())
                {
                    conn.Open();
                    cmdUpdateNama.Connection = conn;
                    cmdUpdateNama.CommandText = "UPDATE detail_pengadaan_produk dp JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK SET dp.NAMA_PRODUK = p.NAMA_PRODUK where dp.ID_PRODUK = p.ID_PRODUK";
                    cmdUpdateNama.ExecuteNonQuery();
                    conn.Close();
                }

                using(MySqlCommand cmdUpdateSatuan = new MySqlCommand())
                {
                    conn.Open();
                    cmdUpdateSatuan.Connection = conn;
                    cmdUpdateSatuan.CommandText = "UPDATE detail_pengadaan_produk dp JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK SET dp.SATUAN = p.SATUAN where dp.ID_PRODUK = p.ID_PRODUK";
                    cmdUpdateSatuan.ExecuteNonQuery();
                    conn.Close();
                }
                this.Close();
            }
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
