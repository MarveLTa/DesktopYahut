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
    /// Interaction logic for CrudUkuranHewan.xaml
    /// </summary>
    public partial class CrudUkuranHewan : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        DateTime tanggal = DateTime.Now;

        public CrudUkuranHewan()
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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                DataGrid gd = (DataGrid)sender;
                DataRowView selected_row = gd.SelectedItem as DataRowView;
                if (selected_row != null)
                {
                    ComboBoxUkuranHewan.SelectedValue = selected_row["NAMA_UKURAN"];
                    IdUkuranHewanText.Text = selected_row["ID_UKURAN_HEWAN"].ToString();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void GetRecords()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select ID_UKURAN_HEWAN, NAMA_UKURAN from ukuran_hewan", conn);
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
                MySqlCommand cmd = new MySqlCommand("select ID_UKURAN_HEWAN, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from ukuran_hewan", conn);
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
            MySqlCommand cmd = new MySqlCommand("select ID_UKURAN_HEWAN, NAMA_UKURAN from ukuran_hewan", conn);
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
            MySqlCommand cmd = new MySqlCommand("select ID_UKURAN_HEWAN, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from ukuran_hewan", conn);
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

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            // Untuk cek jika data sudah terinput sebelumnya
            string ukuran = ((ComboBoxItem)ComboBoxUkuranHewan.SelectedItem).Content.ToString();
            cmd = new MySqlCommand("Select * from ukuran_hewan where NAMA_UKURAN = '" + ukuran + "'", conn); 
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                MessageBox.Show(string.Format("{0} sudah ada!", ukuran), "Warning");
                ClearData();
                return;
                //conn.Close();
            }
            else
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        conn.Open();
                        cmd.CommandText = "INSERT INTO UKURAN_HEWAN(NAMA_UKURAN) VALUES(@ukuran)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (ukuran == "" || ukuran == "-- Pilih --")
                        {
                            MessageBox.Show("Field tidak boleh kosong", "Warning");
                            conn.Close();
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ukuran", ukuran);
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
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Untuk cek jika data sudah terinput sebelumnya
                string ukuran = ((ComboBoxItem)ComboBoxUkuranHewan.SelectedItem).Content.ToString();
                cmd = new MySqlCommand("Select * from ukuran_hewan where NAMA_UKURAN = '" + ukuran + "'", conn);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    MessageBox.Show(string.Format("{0} sudah ada!", ukuran), "Warning");
                    ClearData();
                    return;
                }
                // Cek jika user belum pilih data yang ingin dihapus
                else if (string.IsNullOrEmpty(ComboBoxUkuranHewan.Text) || ComboBoxUkuranHewan.SelectedValue.ToString() == "-- Pilih --")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    conn.Open();
                    ds = new DataSet();
                    string tanggalUpdate = tanggal.ToString("yyyy-MM-dd H:mm:ss");
                    adapter = new MySqlDataAdapter("update ukuran_hewan set NAMA_UKURAN = '" + ComboBoxUkuranHewan.SelectedValue + "', UPDATE_AT = '" + tanggalUpdate + "' where ID_UKURAN_HEWAN = '" + IdUkuranHewanText.Text + "'", conn);

                    if (ComboBoxUkuranHewan.SelectedValue.ToString() == "" || ComboBoxUkuranHewan.SelectedValue.ToString() == "-- Pilih --")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        adapter.Fill(ds, "ukuran_hewan");
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
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    // Cek jika user belum pilih data yang ingin dihapus
                    if (string.IsNullOrEmpty(ComboBoxUkuranHewan.Text) || ComboBoxUkuranHewan.SelectedValue.ToString() == "-- Pilih --")
                    {
                        MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                        return;
                    }
                    else
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM UKURAN_HEWAN WHERE ID_UKURAN_HEWAN = @idukuran";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idukuran", IdUkuranHewanText.Text);
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
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_UKURAN_HEWAN, NAMA_UKURAN from ukuran_hewan", conn);
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

        private void CariUkuranText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("select ID_UKURAN_HEWAN, NAMA_UKURAN from ukuran_hewan where Nama_Ukuran LIKE '" + CariUkuranText.Text + "%'", conn);
                adp.Fill(dt);
                //DataGrid.Items.Refresh();
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void ClearData()
        {
            IdUkuranHewanText.Clear();
            CariUkuranText.Clear();
            ComboBoxUkuranHewan.SelectedIndex = -1;
        }
    }
}
