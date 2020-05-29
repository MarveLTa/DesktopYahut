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
    /// Interaction logic for TambahProdukPengadaan.xaml
    /// </summary>
    public partial class TambahProdukPengadaan : Window
    {

        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string idDetail;

        public TambahProdukPengadaan()
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

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBoxNamaProduk.Text))
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                return;
            }
            else
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = "INSERT INTO DETAIL_PENGADAAN_PRODUK(ID_TRANSAKSIPENGADAAN, ID_PRODUK, JUMLAH) VALUES(@idtransaksi, @idproduk, @jumlah)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@idtransaksi", idTransaksi);
                        cmd.Parameters.AddWithValue("@idproduk", ComboBoxNamaProduk.SelectedValue);
                        cmd.Parameters.AddWithValue("@jumlah", JumlahProdukText.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Berhasil ditambahkan", "Success");

                        using (MySqlCommand cmdUpdateNama = new MySqlCommand())
                        {
                            conn.Open();
                            cmdUpdateNama.Connection = conn;
                            cmdUpdateNama.CommandText = "UPDATE detail_pengadaan_produk dp JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK SET dp.NAMA_PRODUK = p.NAMA_PRODUK where dp.ID_PRODUK = p.ID_PRODUK";
                            cmdUpdateNama.ExecuteNonQuery();
                            conn.Close();
                        }

                        using (MySqlCommand cmdUpdateSatuan = new MySqlCommand())
                        {
                            conn.Open();
                            cmdUpdateSatuan.Connection = conn;
                            cmdUpdateSatuan.CommandText = "UPDATE detail_pengadaan_produk dp JOIN produk p ON dp.ID_PRODUK = p.ID_PRODUK SET dp.SATUAN = p.SATUAN where dp.ID_PRODUK = p.ID_PRODUK";
                            cmdUpdateSatuan.ExecuteNonQuery();
                            conn.Close();
                        }
                        this.Close();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        conn.Close();
                    }
                }

            }
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
