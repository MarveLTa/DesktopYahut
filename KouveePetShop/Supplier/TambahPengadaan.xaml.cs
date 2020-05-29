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
    /// Interaction logic for TambahPengadaan.xaml
    /// </summary>
    public partial class TambahPengadaan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public TambahPengadaan()
        {
            InitializeComponent();

            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);
            conn.Open();
            FillComboBoxNamaProduk();
            FillComboBoxNamaSupplier();
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

        public void FillComboBoxNamaSupplier()
        {
            // Ambil ID dan Nama produk dari tabel produk ke combobox
            string Query = "select ID_SUPPLIER, NAMA_SUPPLIER from petshop.supplier;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idSupplier = reader.GetInt32("ID_SUPPLIER");
                    string namaSupplier = reader.GetString("NAMA_SUPPLIER");
                    ComboBoxNamaSupplier.Items.Add(idSupplier + " - " + namaSupplier);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(ComboBoxNamaProduk.Text) || string.IsNullOrEmpty(ComboBoxNamaSupplier.Text))
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                return;
            }
            else
            {
                try
                {
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "INSERT INTO pengadaan_produk(ID_SUPPLIER, ID_PEGAWAI) VALUES(@idSupplier, @idPegawai)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@idSupplier", ComboBoxNamaSupplier.SelectedValue);
                        cmd.Parameters.AddWithValue("@idPegawai", 1);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Berhasil ditambahkan", "Success");
                    }

                    using (MySqlCommand cmdInputDetail = new MySqlCommand())
                    {
                        conn.Open();
                        cmdInputDetail.CommandText = "INSERT INTO detail_pengadaan_produk(ID_TRANSAKSIPENGADAAN, ID_PRODUK, JUMLAH) values(LAST_INSERT_ID(), @idProduk, @jumlah)";
                        cmdInputDetail.CommandType = CommandType.Text;
                        cmdInputDetail.Connection = conn;

                        cmdInputDetail.Parameters.AddWithValue("@idProduk", ComboBoxNamaProduk.Text);
                        cmdInputDetail.Parameters.AddWithValue("@jumlah", JumlahProdukText.Text);
                        cmdInputDetail.ExecuteNonQuery();
                        conn.Close();
                    }

                    using (MySqlCommand cmdUpdateNomor = new MySqlCommand())
                    {
                        conn.Open();
                        cmdUpdateNomor.Connection = conn;
                        cmdUpdateNomor.CommandText = "UPDATE pengadaan_produk SET NO_TRANSAKSIPENGADAAN = CONCAT('PO','-', DATE_FORMAT(CURRENT_TIMESTAMP, '%Y-%m'), '-', ID_TRANSAKSIPENGADAAN)";
                        cmdUpdateNomor.ExecuteNonQuery();
                        conn.Close();
                    }            

                    using(MySqlCommand cmdUpdateNama = new MySqlCommand())
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

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
