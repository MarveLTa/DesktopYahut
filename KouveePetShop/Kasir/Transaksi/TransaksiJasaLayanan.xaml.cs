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
    /// Interaction logic for TransaksiJasaLayanan.xaml
    /// </summary>
    public partial class TransaksiJasaLayanan : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;

        public TransaksiJasaLayanan()
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

        private void TampilDataGrid()
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("select tr.NO_TRANSAKSI, tr.ID_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, sum(dt.TOTAL) as TOTAL, tr.STATUS_LAYANAN, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2, tr.ID_PEGAWAI2 from transaksi tr JOIN detail_transaksi_jasalayanan dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN JOIN pegawai pg1 on tr.ID_PEGAWAI = pg1.ID_PEGAWAI LEFT JOIN pegawai pg2 on tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER where tr.ID_TRANSAKSI = dt.ID_TRANSAKSI AND tr.NO_TRANSAKSI LIKE 'LY%' GROUP BY tr.NO_TRANSAKSI ORDER BY tr.ID_TRANSAKSI DESC", conn);
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
            conn.Open();
            SubTotal();
            Diskon();
            Total();
            TampilDataGrid();
            conn.Close();
        }

        private void SubTotal()
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE detail_transaksi_jasalayanan dt SET dt.SUB_TOTAL = (SELECT sum(dt.JUMLAH*js.HARGA_JASA_LAYANAN) FROM jasa_layanan js WHERE dt.ID_JASA_LAYANAN = js.ID_JASA_LAYANAN)";
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
                    cmdMember.CommandText = "UPDATE transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET tr.DISKON = (SELECT sum(dt.SUB_TOTAL * 10 / 100) FROM detail_transaksi_jasalayanan dt WHERE dt.ID_TRANSAKSI = tr.ID_TRANSAKSI) WHERE cr.STATUS LIKE 'Member' AND tr.NO_TRANSAKSI LIKE 'LY%'";
                    cmdMember.ExecuteNonQuery();
                }

                using (MySqlCommand cmdNonMember = new MySqlCommand())
                {
                    cmdNonMember.Connection = conn;
                    cmdNonMember.CommandText = "UPDATE transaksi tr JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET tr.DISKON = 0 WHERE cr.STATUS LIKE 'Non Member' AND tr.NO_TRANSAKSI LIKE 'LY%'";
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
                    cmdMember.CommandText = "UPDATE detail_transaksi_jasalayanan dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET dt.TOTAL = (SELECT sum(SUB_TOTAL - (SUB_TOTAL * 10/100)) FROM detail_transaksi_jasalayanan dt2 WHERE dt2.ID_DETAILTRANSAKSI_JASALAYANAN = dt.ID_DETAILTRANSAKSI_JASALAYANAN) WHERE cr.STATUS LIKE 'Member'";
                    cmdMember.ExecuteNonQuery();
                }

                using (MySqlCommand cmdNonMember = new MySqlCommand())
                {
                    cmdNonMember.Connection = conn;
                    cmdNonMember.CommandText = "UPDATE detail_transaksi_jasalayanan dt JOIN transaksi tr ON dt.ID_TRANSAKSI = tr.ID_TRANSAKSI JOIN hewan h ON tr.ID_HEWAN = h.ID_HEWAN JOIN customer cr ON h.ID_CUSTOMER = cr.ID_CUSTOMER SET dt.TOTAL = dt.SUB_TOTAL WHERE cr.STATUS LIKE 'Non Member'";
                    cmdNonMember.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void CariTransaksiJasaLayananText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari transaksi sesuai nama
                // auto mencari data
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("select tr.NO_TRANSAKSI, tr.ID_TRANSAKSI, h.NAMA_HEWAN, tr.DISKON, sum(dt.TOTAL) as TOTAL, tr.STATUS_LAYANAN, tr.STATUS_PEMBAYARAN, tr.ID_PEGAWAI, pg1.NAMA_PEGAWAI as nama1, pg2.NAMA_PEGAWAI as nama2, tr.ID_PEGAWAI2 from transaksi tr " +
                    "JOIN detail_transaksi_jasalayanan dt ON tr.ID_TRANSAKSI = dt.ID_TRANSAKSI " +
                    "JOIN hewan h on tr.ID_HEWAN = h.ID_HEWAN " +
                    "JOIN pegawai pg1 on tr.ID_PEGAWAI = pg1.ID_PEGAWAI " +
                    "LEFT JOIN pegawai pg2 on tr.ID_PEGAWAI2 = pg2.ID_PEGAWAI " +
                    "where tr.NO_TRANSAKSI LIKE 'LY%' AND h.Nama_Hewan LIKE '" + CariTransaksiJasaLayananText.Text + "%'", conn);

                adp.Fill(dt);
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetRecords();
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            UiProsesTransaksiJasaLayanan ui = new UiProsesTransaksiJasaLayanan();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                ui.idTransaksi = selected_row["ID_TRANSAKSI"].ToString();
                ui.NamaHewanText.Text = selected_row["NAMA_HEWAN"].ToString();
                ui.Show();
            }
        }

        /*
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditHewanTransaksi edt = new EditHewanTransaksi();
            DataRowView selected_row = DataGrid.SelectedItem as DataRowView;
            if (selected_row != null)
            {
                edt.idTransaksi = selected_row["ID_TRANSAKSI"].ToString();
                edt.NamaHewanText.Text = selected_row["NAMA_HEWAN"].ToString();

                edt.Show();
            }
        }*/

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
                    string queryDetail = "Delete from detail_transaksi_jasalayanan where ID_TRANSAKSI = @1";
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
    }
}
