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

        public string idTransaksi;
        private TransaksiProduk TP;


        public EditHewanTransaksi()
        {
            InitializeComponent();
            connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
            conn = new MySqlConnection(connection);           
            conn.Open();
            FillComboBoxNamaHewan();
            //FillComboBox();
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

        public void FillComboBox()
        {
            
            MySqlCommand cmd = new MySqlCommand("select h.ID_HEWAN as id, h.NAMA_HEWAN as nama from transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON tr.ID_CUSTOMER = cr.ID_CUSTOMER where tr.ID_TRANSAKSI = '"+idTransaksi+"'", conn);
            
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            da.Fill(dt);

            ComboBoxNamaHewan.ItemsSource = dt.DefaultView;
            ComboBoxNamaHewan.DisplayMemberPath = "nama";
            ComboBoxNamaHewan.SelectedValuePath = "id";
            cmd.Dispose();          
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
                //DataRowView selected_row = tr.DataGrid.SelectedItem as DataRowView;
                adapter = new MySqlDataAdapter("update transaksi set ID_HEWAN = '" + ComboBoxNamaHewan.SelectedValue + "' where ID_TRANSAKSI = '" + idTransaksi + "'", conn);
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TransaksiProduk TP = new TransaksiProduk();
            TP.GetRecords();
        }
    }
}
