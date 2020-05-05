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
    /// Interaction logic for CrudCustomer.xaml
    /// </summary>
    public partial class CrudCustomer : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        DateTime tanggal = DateTime.Now;

        public CrudCustomer()
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

        private void GetRecords()
        {
            conn.Open();
            TampilDataGrid();
            conn.Close();
        }

        private void GetLogsRecords()
        {
            conn.Open();
            TampilDataGridLog();
            conn.Close();
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_CUSTOMER, NAMA_CUSTOMER, ALAMAT_CUSTOMER, TANGGALLAHIR_CUSTOMER, NOTELP_CUSTOMER, STATUS from customer", conn);
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
            MySqlCommand cmd = new MySqlCommand("select ID_CUSTOMER, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from customer", conn);
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

        private void CariCustomerText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari hewan sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("select ID_CUSTOMER, NAMA_CUSTOMER, ALAMAT_CUSTOMER, TANGGALLAHIR_CUSTOMER, NOTELP_CUSTOMER, STATUS from customer where Nama_Customer LIKE '" + CariCustomerText.Text + "%'", conn);
                adp.Fill(dt);
                //DataGrid.Items.Refresh();
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
                    IdCustomerText.Text = selected_row["ID_CUSTOMER"].ToString();
                    NamaCustomerText.Text = selected_row["NAMA_CUSTOMER"].ToString();
                    AlamatCustomerText.Text = selected_row["ALAMAT_CUSTOMER"].ToString();
                    DatePickTglLahir.Text = selected_row["TANGGALLAHIR_CUSTOMER"].ToString();
                    NomorCustomerText.Text = selected_row["NOTELP_CUSTOMER"].ToString();
                    ComboBoxStatus.SelectedValue = selected_row["STATUS"];
                }
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
                    cmd.CommandText = "INSERT INTO CUSTOMER(NAMA_CUSTOMER, ALAMAT_CUSTOMER, TANGGALLAHIR_CUSTOMER, NOTELP_CUSTOMER, STATUS) VALUES(@namacustomer, @alamat, @tanggal, @nomor, @status)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    string status = ((ComboBoxItem)ComboBoxStatus.SelectedItem).Content.ToString();

                    // Cek jika inputan kosong
                    if (string.IsNullOrEmpty(ComboBoxStatus.Text) || NamaCustomerText.Text == "" || DatePickTglLahir.SelectedDate == null || ComboBoxStatus.SelectedIndex == -1 || AlamatCustomerText.Text == "" || NomorCustomerText.Text == "" || status == "-- Pilih --")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {

                        string tanggalLahirCustomer = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");
                        cmd.Parameters.AddWithValue("@namacustomer", NamaCustomerText.Text);
                        cmd.Parameters.AddWithValue("@alamat", AlamatCustomerText.Text);
                        cmd.Parameters.AddWithValue("@tanggal", tanggalLahirCustomer);
                        cmd.Parameters.AddWithValue("@nomor", NomorCustomerText.Text);
                        cmd.Parameters.AddWithValue("@status", status);

                        cmd.ExecuteNonQuery();
                        conn.Close();
                        GetRecords();
                        GetLogsRecords();
                        MessageBox.Show("Berhasil ditambahkan", "Success");
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

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                // Cek jika user belum pilih data yang ingin diedit
                if (IdCustomerText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using(MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            conn.Open();
                            string tanggalUpdate = tanggal.ToString("yyyy-MM-dd H:mm:ss");
                            string status = ((ComboBoxItem)ComboBoxStatus.SelectedItem).Content.ToString();
                            string tanggalLahirCustomer = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");

                            cmd.CommandText = "UPDATE customer set NAMA_CUSTOMER = @namacustomer, ALAMAT_CUSTOMER = @alamat, NOTELP_CUSTOMER = @nomor, TANGGALLAHIR_CUSTOMER = @tanggal, STATUS = @status, UPDATE_AT = @updateAt WHERE ID_CUSTOMER = @idcustomer";
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            // Cek inputan user, kosong atau tidak
                            if (string.IsNullOrEmpty(ComboBoxStatus.Text) || NamaCustomerText.Text == "" || DatePickTglLahir.SelectedDate == null || ComboBoxStatus.SelectedIndex == -1 || AlamatCustomerText.Text == "" || NomorCustomerText.Text == "" || status == "-- Pilih --")
                            {
                                MessageBox.Show("Field tidak boleh kosong", "Warning");
                                conn.Close();
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@idcustomer", IdCustomerText.Text);
                                cmd.Parameters.AddWithValue("@namacustomer", NamaCustomerText.Text);
                                cmd.Parameters.AddWithValue("alamat", AlamatCustomerText.Text);
                                cmd.Parameters.AddWithValue("@nomor", NomorCustomerText.Text);
                                cmd.Parameters.AddWithValue("@tanggal", tanggalLahirCustomer);
                                cmd.Parameters.AddWithValue("@status", ComboBoxStatus.SelectedValue);
                                cmd.Parameters.AddWithValue("@updateAt", tanggalUpdate);

                                cmd.ExecuteNonQuery();
                                conn.Close();
                                GetRecords();
                                GetLogsRecords();
                                MessageBox.Show("Berhasil Diedit!", "Success");
                                ClearData();
                            }
                        }
                        catch(Exception err)
                        {
                            MessageBox.Show(err.Message);
                            conn.Close();
                            return;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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
                // Cek jika user belum memilih data yang ingin dihapus
                if (IdCustomerText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM CUSTOMER WHERE ID_CUSTOMER = @idcustomer";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idcustomer", IdCustomerText.Text);
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
            catch (Exception err)
            {
                if(err is ConstraintException || err is MySqlException)
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
            MySqlCommand cmd = new MySqlCommand("select ID_CUSTOMER, NAMA_CUSTOMER, ALAMAT_CUSTOMER, TANGGALLAHIR_CUSTOMER, NOTELP_CUSTOMER, STATUS from customer", conn);
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
        private void ClearData()
        {
            ComboBoxStatus.SelectedIndex = -1;
            IdCustomerText.Clear();
            NamaCustomerText.Clear();
            AlamatCustomerText.Clear();
            NomorCustomerText.Clear();
            DatePickTglLahir.SelectedDate = null;
        }
    }
}
