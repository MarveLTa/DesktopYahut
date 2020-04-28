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
    /// Interaction logic for CrudPegawai.xaml
    /// </summary>
    public partial class CrudPegawai : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        DateTime tanggal = DateTime.Now;

        public CrudPegawai()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                conn.Open();
                TampilDataGrid();
                TampilDataGridLog();
                conn.Close();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        private void TampilDataGrid()
        {            
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("Select ID_PEGAWAI, ROLE, NAMA_PEGAWAI, ALAMAT_PEGAWAI, TANGGALLAHIR_PEGAWAI, NOTELP_PEGAWAI, USERNAME, PASSWORD from pegawai", conn);
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

        private void TampilDataGridLog()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_PEGAWAI, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from pegawai", conn);
            try
            {
                //conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                LogsDataGrid.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void GetRecords()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("Select ID_PEGAWAI, ROLE, NAMA_PEGAWAI, ALAMAT_PEGAWAI, TANGGALLAHIR_PEGAWAI, NOTELP_PEGAWAI, USERNAME, PASSWORD from pegawai", conn);
                DataGrid.Items.Refresh();
                conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();

                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void GetLogsRecords()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select ID_PEGAWAI, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from pegawai", conn);
                DataGrid.Items.Refresh();
                conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();

                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid gd = (DataGrid)sender;
                DataRowView selected_row = gd.SelectedItem as DataRowView;
                if (selected_row != null)
                {
                    IdPegawaiText.Text = selected_row["ID_PEGAWAI"].ToString();
                    ComboBoxRolePegawai.Text = selected_row["ROLE"].ToString();  
                    NamaPegawaiText.Text = selected_row["NAMA_PEGAWAI"].ToString();
                    AlamatPegawaiText.Text = selected_row["ALAMAT_PEGAWAI"].ToString();
                    NomorPegawaiText.Text = selected_row["NOTELP_PEGAWAI"].ToString();
                    DatePickTglLahir.Text = selected_row["TANGGALLAHIR_PEGAWAI"].ToString();
                    UsernameText.Text = selected_row["USERNAME"].ToString();
                    PasswordText.Text = selected_row["PASSWORD"].ToString();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CariPegawaiText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari pegawai sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("Select ID_PEGAWAI, ROLE,  NAMA_PEGAWAI, ALAMAT_PEGAWAI, TANGGALLAHIR_PEGAWAI, NOTELP_PEGAWAI, USERNAME, PASSWORD from pegawai where Nama_Pegawai LIKE '" + CariPegawaiText.Text + "%'", conn);
                adp.Fill(dt);
                //DataGrid.Items.Refresh();
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using(MySqlCommand cmd = new MySqlCommand())
                {
                    conn.Open();
                    string role = ((ComboBoxItem)ComboBoxRolePegawai.SelectedItem).Content.ToString();
                    cmd.CommandText = "INSERT INTO PEGAWAI(ROLE, NAMA_PEGAWAI, ALAMAT_PEGAWAI, TANGGALLAHIR_PEGAWAI, NOTELP_PEGAWAI, USERNAME, PASSWORD) VALUES(@role, @namapegawai, @alamatpegawai, @tanggal, @nomor, @username, @password)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    // cek jika ada inputan yang kosong
                    if (role == "" || role == "-- Pilih --" || NamaPegawaiText.Text == "" || AlamatPegawaiText.Text == "" || DatePickTglLahir.SelectedDate == null || NomorPegawaiText.Text == "" || UsernameText.Text == "" || PasswordText.Text == "")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        string tanggalLahirPegawai = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");
                        cmd.Parameters.AddWithValue("@role", ComboBoxRolePegawai.SelectedValue);
                        cmd.Parameters.AddWithValue("@namapegawai", NamaPegawaiText.Text);
                        cmd.Parameters.AddWithValue("@alamatpegawai", AlamatPegawaiText.Text);
                        cmd.Parameters.AddWithValue("@tanggal", tanggalLahirPegawai);
                        cmd.Parameters.AddWithValue("@nomor", NomorPegawaiText.Text);
                        cmd.Parameters.AddWithValue("@username", UsernameText.Text);
                        cmd.Parameters.AddWithValue("@password", PasswordText.Text);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        GetRecords();
                        GetLogsRecords();
                        MessageBox.Show("Berhasil ditambahkan", "Success");
                        ClearData();
                    }                  
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
                return;
            }
        }  

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ds = new DataSet();
                string tanggalUpdate = tanggal.ToString("yyyy-MM-dd H:mm:ss");
                // cek jika user belum memilih data yang ingin diedit
                if (IdPegawaiText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                }
                else
                {
                    conn.Open();
                    string tanggalLahirPegawai = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");

                    adapter = new MySqlDataAdapter("update pegawai set ROLE = '" + ComboBoxRolePegawai.SelectedValue + "', NAMA_PEGAWAI = '" + NamaPegawaiText.Text + "', ALAMAT_PEGAWAI = '" + AlamatPegawaiText.Text + "',TANGGALLAHIR_PEGAWAI = '" + tanggalLahirPegawai +"', NOTELP_PEGAWAI = '" + NomorPegawaiText.Text + "', USERNAME = '" + UsernameText.Text + "', PASSWORD = '" + PasswordText.Text +"', UPDATE_AT = '" + tanggalUpdate + "' where ID_PEGAWAI = '" + IdPegawaiText.Text + "'", conn);

                    string role = ((ComboBoxItem)ComboBoxRolePegawai.SelectedItem).Content.ToString();
                    // cek jika ada inputan yang kosong
                    if (ComboBoxRolePegawai.SelectedValue.ToString() == "" || ComboBoxRolePegawai.SelectedValue.ToString() == "-- Pilih --" || ComboBoxRolePegawai.SelectedIndex == -1 || NamaPegawaiText.Text == "" || AlamatPegawaiText.Text == "" || DatePickTglLahir.SelectedDate == null || NomorPegawaiText.Text == "" || UsernameText.Text == "" || PasswordText.Text == "")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        adapter.Fill(ds, "pegawai");
                        conn.Close();
                        GetRecords();
                        GetLogsRecords();
                        MessageBox.Show("Berhasil diedit!", "Success");
                        ClearData();
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
                return;
            }
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            string message = "Apakah anda ingin menghapus data ini ?";
            string caption = "Warning";
            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;

            try
            {
                // cek jika user belum memilih data yang ingin dihapus
                if (IdPegawaiText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM PEGAWAI WHERE ID_PEGAWAI = @idpegawai";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idpegawai", IdPegawaiText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
                            GetLogsRecords();
                            MessageBox.Show("Berhasil Dihapus!", "Success");
                            ClearData();
                        }
                        else
                        {
                            conn.Close();
                            ClearData();
                            return;
                        }
                    }
                }
            }
            catch(Exception err)
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

        private void BtnTampil_Click(object sender, RoutedEventArgs e)
        {
            // Tampil semua data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("Select ID_PEGAWAI, ROLE, NAMA_PEGAWAI, ALAMAT_PEGAWAI, TANGGALLAHIR_PEGAWAI, NOTELP_PEGAWAI, USERNAME, PASSWORD from pegawai", conn);
            try
            {
                conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                conn.Close();

                DataGrid.DataContext = dt;
                ClearData();
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void LogsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ClearData()
        {
            IdPegawaiText.Clear();
            ComboBoxRolePegawai.SelectedIndex = -1;
            NamaPegawaiText.Clear();
            AlamatPegawaiText.Clear();
            NomorPegawaiText.Clear();
            DatePickTglLahir.SelectedDate = null;
            UsernameText.Clear();
            PasswordText.Clear();
        }
    }
}
