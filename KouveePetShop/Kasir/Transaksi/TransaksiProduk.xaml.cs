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
                SubTotal();
                Diskon();
                Total();
                TampilDataGrid();
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public static T IsWindowOpen<T>(string name = null)
    where T : Window
        {
            var windows = Application.Current.Windows.OfType<T>();
            return string.IsNullOrEmpty(name) ? windows.FirstOrDefault() : windows.FirstOrDefault(w => w.Name.Equals(name));
        }


        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select tr.NO_TRANSAKSI, tr.ID_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, dt.TOTAL, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2, tr.ID_PEGAWAI2 from transaksi tr " +
                "JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI " +
                "JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN J" +
                "OIN pegawai pg1 on tr.ID_PEGAWAI = pg1.ID_PEGAWAI " +
                "JOIN pegawai pg2 on tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI " +
                "where tr.NO_TRANSAKSI LIKE 'PR%'", conn);
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

        public void GetRecords()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select tr.NO_TRANSAKSI, tr.ID_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, dt.TOTAL, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2, tr.ID_PEGAWAI2 from transaksi tr " +
                "JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI " +
                "JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN J" +
                "OIN pegawai pg1 on tr.ID_PEGAWAI = pg1.ID_PEGAWAI " +
                "JOIN pegawai pg2 on tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI " +
                "where tr.NO_TRANSAKSI LIKE 'PR%'", conn);
            try
            {
                conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();

                DataGrid.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void SubTotal()
        {
            try
            {
                using(MySqlCommand cmd = new MySqlCommand())
                {                  
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE detail_transaksi_produk dt SET dt.SUB_TOTAL = (SELECT sum(dt.JUMLAH*p.HARGA_PRODUK) FROM produk p WHERE dt.ID_PRODUK = p.ID_PRODUK)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Diskon()
        {
            try
            {
                using (MySqlCommand cmdMember = new MySqlCommand())
                {
                    cmdMember.Connection = conn;
                    cmdMember.CommandText = "UPDATE transaksi tr SET tr.DISKON = (SELECT sum(dt.SUB_TOTAL * (10 / 100)) FROM detail_transaksi_produk dt, customer cr WHERE tr.ID_TRANSAKSI = dt.ID_TRANSAKSI AND cr.STATUS LIKE 'MEMBER')";
                    cmdMember.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Total()
        {
            try
            {
                using (MySqlCommand cmdMember = new MySqlCommand())
                {
                    cmdMember.Connection = conn;
                    cmdMember.CommandText = "UPDATE detail_transaksi_produk dt SET TOTAL = ( SELECT sum(dt.SUB_TOTAL - tr.DISKON) FROM transaksi tr WHERE dt.ID_TRANSAKSI = tr.ID_TRANSAKSI)";
                    cmdMember.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CariTransaksiProdukText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari transaksi sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("select tr.NO_TRANSAKSI, tr.ID_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, dt.TOTAL, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2, tr.ID_PEGAWAI2 from transaksi tr " +
                "JOIN detail_transaksi_produk dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI " +
                "JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN J" +
                "OIN pegawai pg1 on tr.ID_PEGAWAI = pg1.ID_PEGAWAI " +
                "JOIN pegawai pg2 on tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI " +
                "where tr.NO_TRANSAKSI LIKE 'PR%' and h.Nama_Hewan LIKE '" + CariTransaksiProdukText.Text + "%'", conn);

                adp.Fill(dt);
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            UiProsesTransaksi ui = new UiProsesTransaksi();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                ui.NamaHewanText.Text = selected_row["NAMA_HEWAN"].ToString();
                ui.Show();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditHewanTransaksi edt = new EditHewanTransaksi();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if(selected_row != null)
            {
                edt.IdTransaksiText.Text = selected_row["ID_TRANSAKSI"].ToString();
                edt.NamaHewanText.Text = selected_row["NAMA_HEWAN"].ToString();
                edt.Show();     
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
                MySqlCommand cmd2;
                DataRowView row = (DataRowView)((Button)e.Source).DataContext;

                string query = "Delete from transaksi where ID_TRANSAKSI = @1";
                cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add(new MySqlParameter("@1", row["ID_TRANSAKSI"]));

                conn.Open();

                if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                {
                    string queryDetail = "Delete from detail_transaksi_produk where ID_TRANSAKSI = @1";
                    cmd2 = new MySqlCommand(queryDetail, conn);
                    cmd2.Parameters.Add(new MySqlParameter("@1", row["ID_TRANSAKSI"]));
                    cmd2.ExecuteNonQuery();

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
            GetRecords();
        }
    }
}
