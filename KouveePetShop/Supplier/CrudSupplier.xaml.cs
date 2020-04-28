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
    /// Interaction logic for CrudSupplier.xaml
    /// </summary>
    public partial class CrudSupplier : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        DateTime tanggal = DateTime.Now;

        public string role;

        public CrudSupplier()
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
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            } 
        }

        public string GetValueRole(string value)
        {
            string roleValue;

            roleValue = value;
            //cobaText.Text = roleValue;

            return roleValue;
        }

        private void CariSupplierText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari hewan sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("select ID_SUPPLIER, NAMA_SUPPLIER, NOTELP_SUPPLIER, ALAMAT_SUPPLIER from supplier where Nama_Supplier LIKE '" + CariSupplierText.Text + "%'", conn);
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
                    IdSupplierkText.Text = selected_row["ID_SUPPLIER"].ToString();
                    NamaSupplierkText.Text = selected_row["NAMA_SUPPLIER"].ToString();
                    NomorSupplierkText.Text = selected_row["NOTELP_SUPPLIER"].ToString();
                    AlamatSupplierkText.Text = selected_row["ALAMAT_SUPPLIER"].ToString();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_SUPPLIER, NAMA_SUPPLIER, NOTELP_SUPPLIER, ALAMAT_SUPPLIER from supplier", conn);
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
            MySqlCommand cmd = new MySqlCommand("select ID_SUPPLIER, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from supplier", conn);
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
                MySqlCommand cmd = new MySqlCommand("select ID_SUPPLIER, NAMA_SUPPLIER, NOTELP_SUPPLIER, ALAMAT_SUPPLIER from supplier", conn);
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
                MySqlCommand cmd = new MySqlCommand("select ID_SUPPLIER, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from supplier", conn);
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

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = "INSERT INTO SUPPLIER(NAMA_SUPPLIER, NOTELP_SUPPLIER, ALAMAT_SUPPLIER) VALUES(@namasupplier, @nomor, @alamatsupplier)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;


                        // Cek jika inputan kosong
                        if (NamaSupplierkText.Text == "" || NomorSupplierkText.Text == "" || AlamatSupplierkText.Text == "")
                        {
                            MessageBox.Show("Field tidak boleh kosong", "Warning");
                            conn.Close();
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@namasupplier", NamaSupplierkText.Text);
                            cmd.Parameters.AddWithValue("@nomor", NomorSupplierkText.Text);
                            cmd.Parameters.AddWithValue("@alamatsupplier", AlamatSupplierkText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
                            GetLogsRecords();
                            MessageBox.Show("Berhasil ditambahkan", "Success");
                            ClearData();
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ds = new DataSet();
                string tanggalUpdate = tanggal.ToString("yyyy-MM-dd H:mm:ss");
                // Cek jika user belum pilih data yang ingin diedit
                if (IdSupplierkText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    conn.Open();

                    // string role = UiDashboard.passingText;

                   // UiDashboard dashboard = new UiDashboard();
                    //string role = dashboard.RoleText.Text;
                    adapter = new MySqlDataAdapter("update supplier set ID_SUPPLIER = '" + IdSupplierkText.Text + "', NAMA_SUPPLIER = '" + NamaSupplierkText.Text + "', NOTELP_SUPPLIER = '" + NomorSupplierkText.Text + "', ALAMAT_SUPPLIER = '" + AlamatSupplierkText.Text + "', UPDATE_AT = '" + tanggalUpdate + "', CREATED_BY = '" + role + "' where ID_SUPPLIER = '" + IdSupplierkText.Text + "'", conn);

                    // Cek inputan user, kosong atau tidak
                    if (NamaSupplierkText.Text == "" || NomorSupplierkText.Text == "" || AlamatSupplierkText.Text == "")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        adapter.Fill(ds, "supplier");
                        conn.Close();
                        GetRecords();
                        GetLogsRecords();
                        MessageBox.Show("Berhasil Diedit!", "Success");
                        ClearData();
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
                if (IdSupplierkText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM SUPPLIER WHERE ID_SUPPLIER = @idsupplier";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idsupplier", IdSupplierkText.Text);
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
            MySqlCommand cmd = new MySqlCommand("select ID_SUPPLIER, NAMA_SUPPLIER, NOTELP_SUPPLIER, ALAMAT_SUPPLIER from supplier", conn);
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
            IdSupplierkText.Clear();
            NamaSupplierkText.Clear();
            NomorSupplierkText.Clear();
            AlamatSupplierkText.Clear();
        }

        private void LogsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
