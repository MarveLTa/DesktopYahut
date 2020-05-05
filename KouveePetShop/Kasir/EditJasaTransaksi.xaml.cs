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
    /// Interaction logic for EditJasaTransaksi.xaml
    /// </summary>
    public partial class EditJasaTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string jumlah;
        public string idDetail;

        public EditJasaTransaksi()
        {
            InitializeComponent();

            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);
            conn.Open();
            FillComboBoxNamaJasa();
            conn.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        public void FillComboBoxNamaJasa()
        {
            // Ambil ID dan Nama produk dari tabel produk ke combobox
            string Query = "select ID_JASA_LAYANAN, NAMA_JASA_LAYANAN from petshop.jasa_layanan;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idJasa = reader.GetInt32("ID_JASA_LAYANAN");
                    string namajasa = reader.GetString("NAMA_JASA_LAYANAN");
                    ComboBoxJasaLayanan.Items.Add(idJasa + " - " + namajasa);
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
            if (string.IsNullOrEmpty(ComboBoxJasaLayanan.Text))
            {
                MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                return;
            }
            else
            {
                conn.Open();
                TransaksiProduk tr = new TransaksiProduk();
                adapter = new MySqlDataAdapter("update detail_transaksi_jasalayanan dt, transaksi tr SET dt.ID_JASA_LAYANAN = '" + ComboBoxJasaLayanan.SelectedValue + "', dt.JUMLAH = '" + JumlahJasaLayananText.Text + "' WHERE dt.ID_TRANSAKSI = tr.ID_TRANSAKSI AND tr.ID_TRANSAKSI = '" + idTransaksi + "' AND dt.ID_DETAILTRANSAKSI_JASALAYANAN = '" + idDetail + "'", conn);
                adapter.Fill(ds, "detail_transaksi_jasalayanan");
                MessageBox.Show("Edit berhasil!", "Success");
                conn.Close();
                this.Close();
            }
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
