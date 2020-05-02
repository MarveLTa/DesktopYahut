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
    /// Interaction logic for EditProdukTransaksi.xaml
    /// </summary>
    public partial class EditProdukTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string namaHewan;
        public string jumlah;
        public EditProdukTransaksi()
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

        public void FillTextBoxJumlah()
        {
            // Ambil jumlah dari tabel detail transaksi produk ke textbox
            string Query = "select dt.JUMLAH as JUMLAH from detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + IdTransaksiText.Text + "' ";
            MySqlCommand cmdTextBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdTextBox.ExecuteReader();

                while (reader.Read())
                {
                    JumlahProdukText.Text = reader.GetString("JUMLAH");
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
                TransaksiProduk tr = new TransaksiProduk();
                adapter = new MySqlDataAdapter("update detail_transaksi_produk dt, transaksi tr SET dt.ID_PRODUK = '" + ComboBoxNamaProduk.SelectedValue + "' WHERE dt.ID_TRANSAKSI = tr.ID_TRANSAKSI AND tr.ID_TRANSAKSI = '" + IdTransaksiText.Text + "'", conn);
                adapter.Fill(ds, "detail_transaksi_produk");
                MessageBox.Show("Edit berhasil!", "Success");
                conn.Close();
                this.Close();
            }
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnHewan_Click(object sender, RoutedEventArgs e)
        {
            EditHewanTransaksi edt = new EditHewanTransaksi();
            edt.IdTransaksiText.Text = IdTransaksiText.Text;
            edt.NamaHewanText.Text = namaHewan;
            edt.Show();
            this.Close();
        }
    }
}
