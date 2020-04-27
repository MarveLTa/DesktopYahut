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
    /// Interaction logic for CrudHewan.xaml
    /// </summary>
    public partial class CrudHewan : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        public CrudHewan()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                conn.Open();
                FillComboBoxJenisHewan();
                FillComboBoxCustomer();
                TampilDataGrid();
                conn.Close();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        public void FillComboBoxJenisHewan()
        {
            // Ambil ID dan Nama Jenis Hewan dari tabel pegawai ke combobox
            string Query = "select ID_JENIS_HEWAN, NAMA_JENIS from petshop.jenis_hewan;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idJenis = reader.GetInt32("ID_JENIS_HEWAN");
                    string namaJenis = reader.GetString("NAMA_JENIS");
                    ComboBoxIdJenisHewan.Items.Add(idJenis + " - " + namaJenis);
                    // ComboBoxIdPegawai.Items.Add(idPegawai);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        public void FillComboBoxCustomer()
        {
            // Ambil ID dan Nama Customer dari tabel pegawai ke combobox
            string Query = "select ID_CUSTOMER, NAMA_CUSTOMER from petshop.customer;";
            MySqlCommand cmdComboBox = new MySqlCommand(Query, conn);
            MySqlDataReader reader;
            try
            {
                reader = cmdComboBox.ExecuteReader();

                while (reader.Read())
                {
                    int idCustomer = reader.GetInt32("ID_CUSTOMER");
                    string namaCustomer = reader.GetString("NAMA_CUSTOMER");
                    ComboBoxIdCustomer.Items.Add(idCustomer + " - " + namaCustomer);
                    // ComboBoxIdPegawai.Items.Add(idPegawai);
                }
                reader.Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CariHewanText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari hewan sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("Select ID_HEWAN, ID_JENIS_HEWAN, ID_CUSTOMER, NAMA_HEWAN, TANGGALLAHIR_HEWAN from hewan where Nama_Hewan LIKE '" + CariHewanText.Text + "%'", conn);
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
                    IdHewanText.Text = selected_row["ID_HEWAN"].ToString();
                    ComboBoxIdJenisHewan.Text = selected_row["ID_JENIS_HEWAN"].ToString();
                    ComboBoxIdCustomer.Text = selected_row["ID_CUSTOMER"].ToString();
                    NamaHewanText.Text = selected_row["NAMA_HEWAN"].ToString();
                    DatePickTglLahir.Text = selected_row["TANGGALLAHIR_HEWAN"].ToString();
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            } 
        }

        private void GetRecords()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select ID_HEWAN, ID_JENIS_HEWAN, ID_CUSTOMER, NAMA_HEWAN, TANGGALLAHIR_HEWAN from hewan", conn);
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_HEWAN, ID_JENIS_HEWAN, ID_CUSTOMER, NAMA_HEWAN, TANGGALLAHIR_HEWAN from hewan", conn);
            try
            {
                //conn.Open();
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

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = "INSERT INTO HEWAN(ID_JENIS_HEWAN, ID_CUSTOMER, NAMA_HEWAN, TANGGALLAHIR_HEWAN) VALUES(@idjenis, @idcustomer, @hewan, @tanggal)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;


                        // Cek jika inputan kosong
                        if (string.IsNullOrEmpty(ComboBoxIdJenisHewan.Text) || string.IsNullOrEmpty(ComboBoxIdCustomer.Text) || NamaHewanText.Text == "" || DatePickTglLahir.SelectedDate == null || ComboBoxIdCustomer.SelectedIndex == -1 || ComboBoxIdJenisHewan.SelectedIndex == -1)
                        {
                            MessageBox.Show("Field tidak boleh kosong", "Warning");
                            conn.Close();
                        }
                        else
                        {
                            string tanggalLahirHewan = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");
                            cmd.Parameters.AddWithValue("@idjenis", ComboBoxIdJenisHewan.SelectedValue);
                            cmd.Parameters.AddWithValue("@idcustomer", ComboBoxIdCustomer.SelectedValue);
                            cmd.Parameters.AddWithValue("@hewan", NamaHewanText.Text);
                            cmd.Parameters.AddWithValue("@tanggal", tanggalLahirHewan);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
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

                // Cek jika user belum pilih data yang ingin diedit
                if (string.IsNullOrEmpty(ComboBoxIdJenisHewan.Text) || string.IsNullOrEmpty(ComboBoxIdCustomer.Text) || NamaHewanText.Text == "" || DatePickTglLahir.SelectedDate == null)
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    conn.Open();
                    string tanggalLahirHewan = DatePickTglLahir.SelectedDate.Value.ToString("yyyy-MM-dd");

                    adapter = new MySqlDataAdapter("update hewan set ID_JENIS_HEWAN = '" + ComboBoxIdJenisHewan.SelectedValue + "', ID_CUSTOMER = '" + ComboBoxIdCustomer.SelectedValue + "', NAMA_HEWAN = '" + NamaHewanText.Text + "', TANGGALLAHIR_HEWAN = '" + tanggalLahirHewan + "' where ID_HEWAN = '" + IdHewanText.Text + "'", conn);

                    // Cek inputan user, kosong atau tidak
                    if (ComboBoxIdJenisHewan.SelectedIndex == -1 || ComboBoxIdCustomer.SelectedIndex == -1 || NamaHewanText.Text == "" || DatePickTglLahir.SelectedDate == null)
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        adapter.Fill(ds, "hewan");
                        conn.Close();
                        GetRecords();
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
                if (string.IsNullOrEmpty(ComboBoxIdJenisHewan.Text) || string.IsNullOrEmpty(ComboBoxIdCustomer.Text) || NamaHewanText.Text == "" || DatePickTglLahir.SelectedDate == null)
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM HEWAN WHERE ID_HEWAN = @idhewan";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idhewan", IdHewanText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
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
            MySqlCommand cmd = new MySqlCommand("select ID_HEWAN, ID_JENIS_HEWAN, ID_CUSTOMER, NAMA_HEWAN, TANGGALLAHIR_HEWAN from hewan", conn);
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
            ComboBoxIdCustomer.SelectedIndex = -1;
            ComboBoxIdJenisHewan.SelectedIndex = -1;
            IdHewanText.Clear();
            NamaHewanText.Clear();
            DatePickTglLahir.SelectedDate = null;
        }    
    }
}
