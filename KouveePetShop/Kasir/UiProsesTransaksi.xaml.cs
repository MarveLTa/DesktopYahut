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
    /// Interaction logic for UiProsesTransaksi.xaml
    /// </summary>
    public partial class UiProsesTransaksi : Window
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public string idTransaksi;
        public string status;
        public UiProsesTransaksi()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                conn.Open();
                FillComboBoxNamaPegawai();
                conn.Close();

                Loaded += Window_Loaded;
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        public void FillComboBoxNamaPegawai()
        {
            // Ambil ID dan Nama produk dari tabel produk ke combobox
            string Query = "select ID_PEGAWAI, NAMA_PEGAWAI from petshop.pegawai WHERE ROLE = 'Kasir';";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idPegawai = reader.GetInt32("ID_PEGAWAI");
                    string namaPegawai = reader.GetString("NAMA_PEGAWAI");
                    ComboBoxNamaKasir.Items.Add(idPegawai + " - " + namaPegawai);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void FillTextBox()
        {
            string Query = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            try
            {         
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    string nama = (reader["NAMA_CUSTOMER"].ToString());
                    //NamaCustomerText.RefreshCurrent();
                   // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();               
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("SELECT dt.ID_PEMBAYARAN_PRODUK, p.NAMA_PRODUK as nama, dt.JUMLAH as jumlah from detail_transaksi_produk dt JOIN produk p ON dt.ID_PRODUK = p.ID_PRODUK JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "' ", conn);
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

        private void HitungTotalDiskon()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(tr.DISKON) as DISKON from transaksi tr " +
                "JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN " +
                "JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    DiskonText.Text = (reader["DISKON"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void HitungTotalHarga()
        {
            MySqlCommand cmd = new MySqlCommand("SELECT sum(dt.TOTAL) as TOTAL from detail_transaksi_produk dt " +
                "JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI " +
                "JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN " +
                "JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER " +
                "WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    TotalHargaText.Text = (reader["TOTAL"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void CekStatus()
        {
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT tr.STATUS_PEMBAYARAN as STATUS from transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER WHERE tr.STATUS_PEMBAYARAN = 'Success' AND tr.ID_TRANSAKSI = '" + idTransaksi + "'", conn);
                MySqlDataReader reader;

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string statusData = (reader["STATUS"].ToString());
                    status = statusData;
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void BtnBayar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CekStatus();
                conn.Open();
                
                if(status == "Success")
                {
                    MessageBox.Show("Transaksi sudah dibayar!", "Warning");
                    conn.Close();
                    return;
                }
                else
                {
                    if(NamaCustomerText.Text == "")
                    {
                        MessageBox.Show("Silahkan refresh data terlebih dahulu", "Warning");
                        conn.Close();
                    }
                    else if(ComboBoxNamaKasir.SelectedIndex == -1)
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        using (MySqlCommand cmd = new MySqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandText = "UPDATE transaksi tr, customer cr SET tr.STATUS_PEMBAYARAN = 'Success', tr.ID_PEGAWAI2 = '" + ComboBoxNamaKasir.SelectedValue + "' WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "'";
                            cmd.ExecuteNonQuery();

                            UpdateStok();
                            MessageBox.Show("Berhasil dibayar!", "Success");
                            conn.Close();

                            StrukProduk SP = new StrukProduk();
                            SP.idTransaksi = idTransaksi;
                            SP.Show();  
                            this.Close();
                        }
                    }      
                }              
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
            
        }

        private void BtnBatal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();

            // query nama customer
            string Query = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand namaCmd = new MySqlCommand(Query, conn);
            MySqlDataReader reader;

            // query status customer
            string Query2 = "SELECT * FROM customer cr JOIN hewan h on cr.ID_CUSTOMER = h.ID_CUSTOMER " +
                "where h.NAMA_HEWAN LIKE '" + NamaHewanText.Text + "'";
            MySqlCommand statusCmd = new MySqlCommand(Query2, conn);
            MySqlDataReader reader2;
            try
            {
                reader = namaCmd.ExecuteReader();

                while (reader.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    NamaCustomerText.Text = (reader["NAMA_CUSTOMER"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader.Close();

                reader2 = statusCmd.ExecuteReader();

                while (reader2.Read())
                {
                    //txtcomp.Text = (myReader["Comp_Name"].ToString());
                    StatusText.Text = (reader2["STATUS"].ToString());
                    // NamaCustomerText.Text = reader.GetString("cr.NAMA_CUSTOMER");
                }
                reader2.Close();

                SubTotal();
                Diskon();
                Total();
                HitungTotalDiskon();
                HitungTotalHarga();
                TampilDataGrid();                           
                conn.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            TambahProdukTransaksi tp = new TambahProdukTransaksi();
            tp.idTransaksi = idTransaksi;

            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                string idDetail = selected_row["ID_PEMBAYARAN_PRODUK"].ToString();
                tp.idDetail = idDetail;
                tp.Show();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditProdukTransaksi edtPr = new EditProdukTransaksi();
            edtPr.idTransaksi = idTransaksi;

            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                string idDetail = selected_row["ID_PEMBAYARAN_PRODUK"].ToString();
                edtPr.idDetail = idDetail;

                // Ambil jumlah dari tabel detail transaksi produk ke textbox
                conn.Open();
                // jumlah
                string Query = "select dt.JUMLAH as JUMLAH from detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "' AND dt.ID_PEMBAYARAN_PRODUK = '" + idDetail + "'";
                MySqlCommand cmdTextBox = new MySqlCommand(Query, conn);
                MySqlDataReader reader;

                // nama produk
                string Query2 = "select p.NAMA_PRODUK as NAMA from produk p JOIN detail_transaksi_produk dt ON p.ID_PRODUK = dt.ID_PRODUK JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI WHERE tr.ID_TRANSAKSI = '" + idTransaksi + "' AND dt.ID_PEMBAYARAN_PRODUK = '" + idDetail + "'";
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

                string queryDetail = "Delete from detail_transaksi_produk where ID_PEMBAYARAN_PRODUK = @1";
                cmd = new MySqlCommand(queryDetail, conn);
                cmd.Parameters.Add(new MySqlParameter("@1", row["ID_PEMBAYARAN_PRODUK"]));
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

        private void SubTotal()
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE detail_transaksi_produk dt SET dt.SUB_TOTAL = (SELECT sum(dt.JUMLAH*p.HARGA_PRODUK) FROM produk p WHERE dt.ID_PRODUK = p.ID_PRODUK)";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception err)
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
                    cmdMember.CommandText = "UPDATE transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET tr.DISKON = (SELECT sum(dt.SUB_TOTAL * 10 / 100) FROM detail_transaksi_produk dt WHERE dt.ID_TRANSAKSI = tr.ID_TRANSAKSI) WHERE cr.STATUS LIKE 'Member' AND tr.NO_TRANSAKSI LIKE 'PR%'";
                    cmdMember.ExecuteNonQuery();
                }

                using (MySqlCommand cmdNonMember = new MySqlCommand())
                {
                    cmdNonMember.Connection = conn;
                    cmdNonMember.CommandText = "UPDATE transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET tr.DISKON = 0 WHERE cr.STATUS LIKE 'Non Member' AND tr.NO_TRANSAKSI LIKE 'PR%'";
                    cmdNonMember.ExecuteNonQuery();
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
                    cmdMember.CommandText = "UPDATE detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET dt.TOTAL = (SELECT sum(SUB_TOTAL - (SUB_TOTAL * 10/100)) FROM detail_transaksi_produk dt2 WHERE dt2.ID_PEMBAYARAN_PRODUK = dt.ID_PEMBAYARAN_PRODUK) WHERE cr.STATUS LIKE 'Member'";
                    cmdMember.ExecuteNonQuery();
                }

                using (MySqlCommand cmdNonMember = new MySqlCommand())
                {
                    cmdNonMember.Connection = conn;
                    cmdNonMember.CommandText = "UPDATE detail_transaksi_produk dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET dt.TOTAL = dt.SUB_TOTAL WHERE cr.STATUS LIKE 'Non Member'";
                    cmdNonMember.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void UpdateStok()
        {
            try
            {
                using (MySqlCommand cmdMember = new MySqlCommand())
                {
                    cmdMember.Connection = conn;
                    cmdMember.CommandText = "UPDATE produk p JOIN detail_transaksi_produk dt ON p.ID_PRODUK = dt.ID_PRODUK JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI SET p.JUMLAH_PRODUK = (SELECT SUM(JUMLAH_PRODUK - dt.JUMLAH) FROM produk p2 WHERE p2.ID_PRODUK = p.ID_PRODUK) WHERE dt.ID_TRANSAKSI = '" + idTransaksi + "'";
                    cmdMember.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BtnRefresh_Click(sender, e);
        }       
    }
}