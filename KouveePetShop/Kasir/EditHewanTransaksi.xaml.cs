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
    /// Interaction logic for EditHewanTransaksi.xaml
    /// </summary>
    public partial class EditHewanTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        //DataRowView row;

        public EditHewanTransaksi()
        {
            InitializeComponent();

            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);
            conn.Open();
            FillComboBoxNamaHewan();
            conn.Close();
        }

        public void FillComboBoxNamaHewan()
        {
            // Ambil ID dan Nama Hewan dari tabel hewan ke combobox
            string Query = "select ID_HEWAN, NAMA_HEWAN from petshop.hewan;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idHewan = reader.GetInt32("ID_HEWAN");
                    string namaHewan = reader.GetString("NAMA_HEWAN");
                    ComboBoxNamaHewan.Items.Add(idHewan + " - " + namaHewan);
                    // ComboBoxIdPegawai.Items.Add(idPegawai);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBoxNamaHewan.Text))
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                return;
            }
            else
            {
                conn.Open();
                TransaksiProduk tr = new TransaksiProduk();
                //DataRowView selected_row = tr.DataGrid.SelectedItem as DataRowView;
                adapter = new MySqlDataAdapter("update transaksi set ID_HEWAN = '" + ComboBoxNamaHewan.SelectedValue + "' where ID_TRANSAKSI = '" + IdTransaksiText.Text + "'", conn);
                adapter.Fill(ds, "transaksi");
                MessageBox.Show("Edit berhasil!", "Success");
                conn.Close();
                this.Close();
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

        private void BtnProduk_Click(object sender, RoutedEventArgs e)
        {
            EditProdukTransaksi edtPr = new EditProdukTransaksi();
            edtPr.IdTransaksiText.Text = IdTransaksiText.Text;

            // Ambil jumlah dari tabel detail transaksi produk ke textbox
            conn.Open();
            // jumlah
            string Query = "select dt.JUMLAH as JUMLAH from detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + IdTransaksiText.Text + "' ";
            MySqlCommand cmdTextBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            // nama produk
            string Query2 = "select p.NAMA_PRODUK as NAMA from produk p JOIN detail_transaksi_produk dt ON p.ID_PRODUK = dt.ID_PRODUK JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + IdTransaksiText.Text + "' ";
            MySqlCommand cmdTextBox2 = new MySqlCommand(Query2, conn);
            MySqlDataReader reader2;

            try
            {
                // kirim value jumlah ke window berikutnya
                reader = cmdTextBox.ExecuteReader();
                while (reader.Read())
                {
                    edtPr.JumlahProdukText.Text = reader.GetString("JUMLAH");
                }
                reader.Close();

                // kirim value nama produk ke window berikutnya
                reader2 = cmdTextBox2.ExecuteReader();
                while (reader2.Read())
                {
                    edtPr.NamaProdukText.Text = reader2.GetString("NAMA");
                }
                reader2.Close();

                edtPr.namaHewan = NamaHewanText.Text;
                conn.Close();
                edtPr.Show();
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
