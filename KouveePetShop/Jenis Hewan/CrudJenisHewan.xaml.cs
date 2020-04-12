﻿using System;
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

        public CrudJenisHewan()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                conn.Open();
                TampilDataGrid();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView selected_row = gd.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                NamaJenisHewanText.Text = selected_row["NAMA_JENIS"].ToString();
                IdJenisHewanText.Text = selected_row["ID_JENIS_HEWAN"].ToString();
            }
        }

        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            /*
            string jenishewan = NamaJenisHewanText.Text;

            if(jenishewan == "")
            {
                MessageBox.Show("Masukkan data pada field yang tersedia!");
            }
            else
            {
                bool a = tambahJenisHewan(jenishewan);
                try
                {
                    if (a)
                    {
                        MessageBox.Show("Berhasil Diinputkan!");
                        conn.Close();
                    }
                }
                catch(Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }*/
            using (MySqlCommand cmd = new MySqlCommand())
            {
                try
                {
                    conn.Open();
                    cmd.CommandText = "INSERT INTO JENIS_HEWAN(NAMA_JENIS) VALUES(@jenis)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@jenis", NamaJenisHewanText.Text);


                    cmd.ExecuteNonQuery();
                    conn.Close();
                    GetRecords();
                    MessageBox.Show("Berhasil ditambahkan");
                    NamaJenisHewanText.Clear();
                    // conn.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                conn.Open();
                ds = new DataSet();
                adapter = new MySqlDataAdapter("update jenis_hewan set NAMA_JENIS = '" + NamaJenisHewanText.Text + "'where ID_JENIS_HEWAN = '" + IdJenisHewanText.Text + "'", conn);
                adapter.Fill(ds, "jenis_hewan");
                conn.Close();
                GetRecords();
                MessageBox.Show("Berhasil Diedit!");
                NamaJenisHewanText.Clear();
                IdJenisHewanText.Clear();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void BtnHapus_Click(object sender, RoutedEventArgs e)
        {
            conn.Open();
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.CommandText = "DELETE FROM JENIS_HEWAN WHERE ID_JENIS_HEWAN = @idjenis";
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = conn;

                    cmd.Parameters.AddWithValue("@idjenis", IdJenisHewanText.Text);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    GetRecords();
                    MessageBox.Show("Berhasil Dihapus!");
                    NamaJenisHewanText.Clear();
                    IdJenisHewanText.Clear();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
                conn.Close();
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
                conn.Close();

                DataGrid.DataContext = dt;
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
            }
        }


        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Fungsi untuk mencari pegawai sesuai nama
            DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("Nama_Jenis"));
            //DataView dv = new DataView(dt);
            //dv.RowFilter = string.Format("Select * from jenis_hewan where Nama_Jenis LIKE '%{0}%'", NamaJenisHewanText.Text);
            MySqlDataAdapter adp = new MySqlDataAdapter("Select ID_JENIS_HEWAN, NAMA_JENIS from jenis_hewan where Nama_Jenis LIKE '" + NamaJenisHewanText.Text + "%'", conn);
            adp.Fill(dt);
            //DataGrid.Items.Refresh();
            DataGrid.DataContext = dt;
        }

        private void BtnTampil_Click(object sender, RoutedEventArgs e)
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
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
            }
        }

        private void GetRecords()
        {
            MySqlCommand cmd = new MySqlCommand("select ID_JENIS_HEWAN, NAMA_JENIS from jenis_hewan", conn);
            DataGrid.Items.Refresh();
            conn.Open();
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            conn.Close();

            DataGrid.DataContext = dt;
        }
    }
}
