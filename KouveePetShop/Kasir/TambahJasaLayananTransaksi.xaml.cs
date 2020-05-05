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
    /// Interaction logic for TambahJasaLayananTransaksi.xaml
    /// </summary>
    public partial class TambahJasaLayananTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string idDetail;
        public TambahJasaLayananTransaksi()
        {
            InitializeComponent();

            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);
            conn.Open();
            FillComboBoxNamaJasa();
            conn.Close();
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

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ComboBoxJasaLayanan.Text))
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
                        cmd.CommandText = "INSERT INTO DETAIL_TRANSAKSI_JASALAYANAN(ID_TRANSAKSI, ID_JASA_LAYANAN, JUMLAH) VALUES(@idtransaksi, @idjasa, @jumlah)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        cmd.Parameters.AddWithValue("@idtransaksi", idTransaksi);
                        cmd.Parameters.AddWithValue("@idjasa", ComboBoxJasaLayanan.SelectedValue);
                        cmd.Parameters.AddWithValue("@jumlah", JumlahJasaLayananText.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("Berhasil ditambahkan", "Success");
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
