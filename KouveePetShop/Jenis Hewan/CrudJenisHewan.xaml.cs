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
    /// Interaction logic for CrudJenisHewan.xaml
    /// </summary>
    public partial class CrudJenisHewan : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;

        DateTime tanggal = DateTime.Now;

        public CrudJenisHewan()
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
                    NamaJenisHewanText.Text = selected_row["NAMA_JENIS"].ToString();
                    IdJenisHewanText.Text = selected_row["ID_JENIS_HEWAN"].ToString();
                }
            }
            catch(Exception err)
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
                        cmd.CommandText = "INSERT INTO JENIS_HEWAN(NAMA_JENIS) VALUES(@jenis)";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if(NamaJenisHewanText.Text == "")
                        {
                            MessageBox.Show("Field tidak boleh kosong", "Warning");
                            conn.Close();
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@jenis", NamaJenisHewanText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
                            GetLogsRecords();
                            MessageBox.Show("Berhasil ditambahkan");
                            NamaJenisHewanText.Clear();
                        }
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }           
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(NamaJenisHewanText.Text == "")
                {
                    MessageBox.Show("Field tidak boleh kosong", "Warning");
                }
                else
                {
                    conn.Open();
                    ds = new DataSet();
                    string tanggalUpdate = tanggal.ToString("yyyy-MM-dd H:mm:ss");

                    adapter = new MySqlDataAdapter("update jenis_hewan set NAMA_JENIS = '" + NamaJenisHewanText.Text + "', UPDATE_AT = '" + tanggalUpdate + "' where ID_JENIS_HEWAN = '" + IdJenisHewanText.Text + "'", conn);

                    if (NamaJenisHewanText.Text == "")
                    {
                        MessageBox.Show("Field tidak boleh kosong", "Warning");
                        conn.Close();
                    }
                    else
                    {
                        adapter.Fill(ds, "jenis_hewan");
                        conn.Close();
                        GetRecords();
                        GetLogsRecords();
                        MessageBox.Show("Berhasil Diedit!");
                        NamaJenisHewanText.Clear();
                        IdJenisHewanText.Clear();
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
                if(NamaJenisHewanText.Text == "")
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM JENIS_HEWAN WHERE ID_JENIS_HEWAN = @idjenis";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idjenis", IdJenisHewanText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
                            GetLogsRecords();
                            MessageBox.Show("Berhasil Dihapus!");
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
            try
            {
                // Tampil data ke dataGrid
                MySqlCommand cmd = new MySqlCommand("select ID_JENIS_HEWAN, NAMA_JENIS from jenis_hewan", conn);
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
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select ID_JENIS_HEWAN, NAMA_JENIS from jenis_hewan", conn);
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
            MySqlCommand cmd = new MySqlCommand("select ID_JENIS_HEWAN, CREATED_AT, UPDATE_AT, DELETE_AT, CREATED_BY, UPDATED_BY from jenis_hewan", conn);
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

        private void CariJenisHewanText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari pegawai sesuai nama
                DataTable dt = new DataTable();
                //dt.Columns.Add(new DataColumn("Nama_Jenis"));
                //DataView dv = new DataView(dt);
                //dv.RowFilter = string.Format("Select * from jenis_hewan where Nama_Jenis LIKE '%{0}%'", NamaJenisHewanText.Text);
                MySqlDataAdapter adp = new MySqlDataAdapter("Select ID_JENIS_HEWAN, NAMA_JENIS from jenis_hewan where Nama_Jenis LIKE '" + CariJenisHewanText.Text + "%'", conn);
                adp.Fill(dt);
                //DataGrid.Items.Refresh();
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
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

        private void ClearData()
        {
            NamaJenisHewanText.Clear();
            IdJenisHewanText.Clear();
        }  
    }
}
