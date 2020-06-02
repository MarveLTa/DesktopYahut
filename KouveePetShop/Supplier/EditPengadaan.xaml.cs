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
    /// Interaction logic for EditPengadaan.xaml
    /// </summary>
    public partial class EditPengadaan : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string namaSupplier;

        public EditPengadaan()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
            }
            catch(MySqlException err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT ID_DETAIL_PENGADAAN, NAMA_PRODUK, JUMLAH FROM detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN WHERE dp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'", conn);
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
                conn.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnKeluar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            TambahProdukPengadaan TP = new TambahProdukPengadaan();
            TP.idTransaksi = idTransaksi;

            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if(selected_row != null)
            {
                string idDetail = selected_row["ID_DETAIL_PENGADAAN"].ToString();
                TP.idDetail = idDetail;
                TP.Show();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditProdukPengadaan edtPr = new EditProdukPengadaan();
            edtPr.idTransaksi = idTransaksi;

            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if(selected_row != null)
            {
                string idDetail = selected_row["ID_DETAIL_PENGADAAN"].ToString();
                edtPr.idDetail = idDetail;

                // Ambil nama dan jumlah produk ke textbox di editprodukpengadaan
                conn.Open();

                // nama produk
                string Query = "select dp.NAMA_PRODUK from detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN WHERE dp.ID_DETAIL_PENGADAAN = '" + idDetail + "' AND pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
                MySqlCommand cmdTextBox = new MySqlCommand(Query, conn);
                MySqlDataReader reader;

                // jumlah
                string Query2 = "select dp.JUMLAH from detail_pengadaan_produk dp JOIN pengadaan_produk pp ON dp.ID_TRANSAKSIPENGADAAN = pp.ID_TRANSAKSIPENGADAAN WHERE dp.ID_DETAIL_PENGADAAN = '" + idDetail + "' AND pp.ID_TRANSAKSIPENGADAAN = '" + idTransaksi + "'";
                MySqlCommand cmdTextBox2 = new MySqlCommand(Query2, conn);
                MySqlDataReader reader2;

                try
                {                   
                    // kirim value nama produk ke window berikutnya
                    reader = cmdTextBox.ExecuteReader();
                    while (reader.Read())
                    {
                        edtPr.NamaProdukText.Text = reader.GetString("NAMA_PRODUK");
                    }
                    reader.Close();

                    // kirim value jumlah ke window berikutnya
                    reader2 = cmdTextBox2.ExecuteReader();
                    while (reader2.Read())
                    {
                        edtPr.JumlahProdukText.Text = reader2.GetString("JUMLAH");
                    }
                    reader2.Close();

                    conn.Close();
                    edtPr.Show();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                    conn.Close();
                }
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
                DataRowView row = (DataRowView)((Button)e.Source).DataContext;

                string queryDetail = "Delete from detail_pengadaan_produk where ID_DETAIL_PENGADAAN = @1";
                cmd = new MySqlCommand(queryDetail, conn);
                cmd.Parameters.Add(new MySqlParameter("@1", row["ID_DETAIL_PENGADAAN"]));
                conn.Open();

                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {
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

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            TampilDataGrid();
            conn.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }  
    }
}
