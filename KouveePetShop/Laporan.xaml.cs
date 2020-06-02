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

namespace KouveePetShop
{
    /// <summary>
    /// Interaction logic for Laporan.xaml
    /// </summary>
    public partial class Laporan : UserControl
    {
        private int temp;
        public Laporan()
        {
            InitializeComponent();
        }

        private void BtnLaporanProdukTerlaris_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 1;
        }

        private void BtnLaporanJasaLayananTerlaris_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 2;
        }

        private void BtnLaporanPendapatanTahunan_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 3;
        }

        private void BtnLaporanPendapatanBulanan_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            ComboBoxBulan.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 4;
        }

        private void BtnLaporanPengadaanTahunan_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 5;
        }

        private void BtnLaporanPengadaanBulanan_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Hidden;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Hidden;
            ComboBoxTahun.Visibility = Visibility.Visible;
            ComboBoxBulan.Visibility = Visibility.Visible;
            BtnNext.Visibility = Visibility.Visible;
            BtnBack.Visibility = Visibility.Visible;
            temp = 6;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            string tahun = ((ComboBoxItem)ComboBoxTahun.SelectedItem).Content.ToString();
            string bulan = ((ComboBoxItem)ComboBoxBulan.SelectedItem).Content.ToString();
            if (tahun == "")
            {
                MessageBox.Show("Silahkan pilih tahun terlebih dahulu", "Warning");
            }
            else
            {
                if(temp == 1)
                {
                    LaporanProdukTerlaris TP = new LaporanProdukTerlaris();
                    TP.TahunText.Text = tahun;
                    TP.Show();
                }
                else if(temp == 2)
                {
                    LaporanJasaLayananTerlaris TP = new LaporanJasaLayananTerlaris();
                    TP.TahunText.Text = tahun;
                    TP.Show();
                }
                else if(temp == 3)
                {
                    LaporanPendapatanTahunan TP = new LaporanPendapatanTahunan();
                    TP.TahunText.Text = tahun;
                    TP.Show();
                }
                else if(temp == 4)
                {
                    LaporanPendapatanBulanan TP = new LaporanPendapatanBulanan();
                    TP.TahunText.Text = tahun;
                    TP.BulanText.Text = bulan;
                    TP.Show();
                }
                else if(temp == 5)
                {
                    LaporanPengadaanTahunan TP = new LaporanPengadaanTahunan();
                    TP.TahunText.Text = tahun;
                    TP.Show();
                }
                else if(temp == 6)
                {
                    LaporanPengadaanBulanan TP = new LaporanPengadaanBulanan();
                    TP.BulanText.Text = bulan;
                    TP.TahunText.Text = tahun;
                    TP.Show();
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            BtnLaporanProdukTerlaris.Visibility = Visibility.Visible;
            BtnLaporanJasaLayananTerlaris.Visibility = Visibility.Visible;
            BtnLaporanPendapatanBulanan.Visibility = Visibility.Visible;
            BtnLaporanPendapatanTahunan.Visibility = Visibility.Visible;
            BtnLaporanPengadaanBulanan.Visibility = Visibility.Visible;
            BtnLaporanPengadaanTahunan.Visibility = Visibility.Visible;
            ComboBoxTahun.Visibility = Visibility.Hidden;
            ComboBoxBulan.Visibility = Visibility.Hidden;
            BtnNext.Visibility = Visibility.Hidden;
            BtnBack.Visibility = Visibility.Hidden;
        }
    }
}
