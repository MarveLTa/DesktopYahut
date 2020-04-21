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
using System.IO;
using Microsoft.Win32;
using System.Drawing;
using Image = System.Drawing.Image;

namespace KouveePetShop
{
    /// <summary>
    /// Interaction logic for CrudProduk.xaml
    /// </summary>
    public partial class CrudProduk : UserControl
    {
        MySqlDataAdapter adapter = new MySqlDataAdapter();
        public DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        private string connection;
        MySqlConnection conn;
        MySqlCommand cmd;


        public CrudProduk()
        {
            InitializeComponent();

            try
            {
                connection = "Server=localhost; User Id=root;Password=;Database=petshop;Allow Zero Datetime=True";
                conn = new MySqlConnection(connection);
                TampilDataGrid();
            }
            catch (MySqlException e)
            {
                MessageBox.Show(e.Message, "Warning");
                return;
            }
        }

        private void TampilDataGrid()
        {
            conn.Open();
            ds = new DataSet();
            try
            {
                // Tampil data ke dataGrid
                adapter = new MySqlDataAdapter("Select ID_PRODUK as 'ID PRODUK', NAMA_PRODUK as 'NAMA PRODUK', HARGA_PRODUK as 'HARGA PRODUK', SATUAN, JUMLAH_PRODUK AS 'JUMLAH PRODUK', JUMLAH_MINIMUM_PRODUK AS 'JUMLAH MINIMUM PRODUK', GAMBAR_PRODUK AS 'GAMBAR PRODUK' from produk", conn);
                adapter.Fill(ds, "produk");
                conn.Close();
                GetRecords();
            }
            catch (MySqlException d)
            {
                MessageBox.Show(d.Message);
                conn.Close();
                return;
            }
        }

        private void CariProdukText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Fungsi untuk mencari produk sesuai nama
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter("Select ID_PRODUK as 'ID PRODUK', NAMA_PRODUK as 'NAMA PRODUK', HARGA_PRODUK as 'HARGA PRODUK', SATUAN, JUMLAH_PRODUK AS 'JUMLAH PRODUK', JUMLAH_MINIMUM_PRODUK AS 'JUMLAH MINIMUM PRODUK', GAMBAR_PRODUK AS 'GAMBAR PRODUK' from produk where Nama_Produk LIKE '" + CariProdukText.Text + "%'", conn);
                adp.Fill(dt);
                DataGrid.DataContext = dt;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }


        private void BtnTambah_Click(object sender, RoutedEventArgs e)
        {
            int parsedValue;
            double parseValue;

            if (NamaProdukText.Text == "" || String.IsNullOrEmpty(HargaProdukText.ToString()) || String.IsNullOrEmpty(JumlahProdukText.ToString()) || String.IsNullOrEmpty(JumlahMinimumProdukText.ToString()) || LokasiGambarText.Text == "" || SatuanText.Text == "")
            {
                MessageBox.Show("Field tidak boleh kosong!", "Warning");
                return;
            }
            else if (!double.TryParse(HargaProdukText.Text, out parseValue))
            {
                // cek jika inputan bukan angka
                MessageBox.Show("Harga Produk hanya boleh angka!");
                return;
            }
            else if (!int.TryParse(JumlahProdukText.Text, out parsedValue))
            {
                MessageBox.Show("Jumlah Produk hanya boleh angka!");
                return;
            }
            else if (!int.TryParse(JumlahMinimumProdukText.Text, out parsedValue))
            {
                MessageBox.Show("Jumlah Minimum Produk hanya boleh angka!");
                return;
            }
            else
            {
                // convert string ke int
                int jumlahProduk = int.Parse(JumlahProdukText.Text);
                int jumlahMinimum = int.Parse(JumlahMinimumProdukText.Text);

                if (jumlahMinimum > jumlahProduk)
                {
                    // cek jika jumlah minimum lebih besar dari jumlah produk
                    MessageBox.Show("Jumlah Minimum Produk harus lebih kecil dari Jumlah Produk!");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            byte[] picBytes = null;
                            FileStream fs = new FileStream(this.LokasiGambarText.Text, FileMode.Open, FileAccess.Read);
                            BinaryReader br = new BinaryReader(fs);
                            picBytes = br.ReadBytes((int)fs.Length);
                            conn.Open();

                            cmd.CommandText = "INSERT INTO PRODUK(NAMA_PRODUK, HARGA_PRODUK, SATUAN, JUMLAH_PRODUK, JUMLAH_MINIMUM_PRODUK, GAMBAR_PRODUK) VALUES(@namaproduk, @hargaproduk, @satuan, @jumlahproduk, @jumlahminimum, @gambarproduk)";
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            if (NamaProdukText.Text == "" || String.IsNullOrEmpty(HargaProdukText.ToString()) || String.IsNullOrEmpty(JumlahProdukText.ToString()) || String.IsNullOrEmpty(JumlahMinimumProdukText.ToString()) || LokasiGambarText.Text == "" || SatuanText.Text == "")
                            {
                                MessageBox.Show("Field tidak boleh kosong!", "Warning");
                                conn.Close();
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@namaproduk", NamaProdukText.Text);
                                cmd.Parameters.AddWithValue("@hargaproduk", HargaProdukText.Text);
                                cmd.Parameters.AddWithValue("@satuan", SatuanText.Text);
                                cmd.Parameters.AddWithValue("@jumlahproduk", JumlahProdukText.Text);
                                cmd.Parameters.AddWithValue("@jumlahminimum", JumlahMinimumProdukText.Text);
                                cmd.Parameters.AddWithValue("@gambarproduk", picBytes);

                                cmd.ExecuteNonQuery();
                                conn.Close();
                                GetRecords();
                                MessageBox.Show("Berhasil ditambahkan");
                                ClearData();
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            conn.Close();
                            return;
                        }
                    }
                }
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            int parsedValue;
            double parseValue;

            if (NamaProdukText.Text == "" || String.IsNullOrEmpty(HargaProdukText.ToString()) || String.IsNullOrEmpty(JumlahProdukText.ToString()) || String.IsNullOrEmpty(JumlahMinimumProdukText.ToString()) || SatuanText.Text == "")
            {
                MessageBox.Show("Field tidak boleh kosong!", "Warning");
                return;
            }
            else if (!double.TryParse(HargaProdukText.Text, out parseValue))
            {
                // cek jika inputan bukan angka
                MessageBox.Show("Harga Produk hanya boleh angka!");
                return;
            }
            else if (!int.TryParse(JumlahProdukText.Text, out parsedValue))
            {
                MessageBox.Show("Jumlah Produk hanya boleh angka!");
                return;
            }
            else if (!int.TryParse(JumlahMinimumProdukText.Text, out parsedValue))
            {
                MessageBox.Show("Jumlah Minimum Produk hanya boleh angka!");
                return;
            }
            else
            {
                // convert string ke int
                int jumlahProduk = int.Parse(JumlahProdukText.Text);
                int jumlahMinimum = int.Parse(JumlahMinimumProdukText.Text);
                if (jumlahMinimum > jumlahProduk)
                {
                    MessageBox.Show("Jumlah Minimum Produk harus lebih kecil dari Jumlah Produk!");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        try
                        {
                            byte[] picBytes = null;
                            FileStream fs = new FileStream(this.LokasiGambarText.Text, FileMode.Open, FileAccess.Read);
                            BinaryReader br = new BinaryReader(fs);
                            picBytes = br.ReadBytes((int)fs.Length);
                            conn.Open();

                            cmd.CommandText = "UPDATE produk set NAMA_PRODUK = @namaproduk, HARGA_PRODUK = @hargaproduk, SATUAN = @satuan, JUMLAH_PRODUK = @jumlahproduk, JUMLAH_MINIMUM_PRODUK = @jumlahminimum, GAMBAR_PRODUK = @gambarproduk WHERE ID_PRODUK = @idproduk";
                            cmd.CommandType = CommandType.Text;
                            cmd.Connection = conn;

                            if (NamaProdukText.Text == "" || String.IsNullOrEmpty(HargaProdukText.ToString()) || String.IsNullOrEmpty(JumlahProdukText.ToString()) || String.IsNullOrEmpty(JumlahMinimumProdukText.ToString()) || SatuanText.Text == "")
                            {
                                MessageBox.Show("Field tidak boleh kosong!", "Warning");
                                conn.Close();
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@idproduk", IdProdukText.Text);
                                cmd.Parameters.AddWithValue("@namaproduk", NamaProdukText.Text);
                                cmd.Parameters.AddWithValue("@hargaproduk", HargaProdukText.Text);
                                cmd.Parameters.AddWithValue("satuan", SatuanText.Text);
                                cmd.Parameters.AddWithValue("@jumlahproduk", JumlahProdukText.Text);
                                cmd.Parameters.AddWithValue("@jumlahminimum", JumlahMinimumProdukText.Text);
                                cmd.Parameters.AddWithValue("@gambarproduk", picBytes);

                                cmd.ExecuteNonQuery();
                                conn.Close();
                                GetRecords();
                                MessageBox.Show("Berhasil Diedit!");
                                ClearData();
                            }
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(err.Message);
                            conn.Close();
                            return;
                        }
                    }
                }
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
                if (NamaProdukText.Text == "" || String.IsNullOrEmpty(HargaProdukText.ToString()) || String.IsNullOrEmpty(JumlahProdukText.ToString()) || String.IsNullOrEmpty(JumlahMinimumProdukText.ToString()))
                {
                    MessageBox.Show("Silahkan pilih data terlebih dahulu", "Warning");
                    return;
                }
                else
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        conn.Open();
                        cmd.CommandText = "DELETE FROM PRODUK WHERE ID_PRODUK = @idproduk";
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;

                        if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                        {
                            cmd.Parameters.AddWithValue("@idproduk", IdProdukText.Text);
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            GetRecords();
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
                MessageBox.Show(err.Message);
            }
        }

        private void BtnTampil_Click(object sender, RoutedEventArgs e)
        {
            // Tampil data ke dataGrid
            MySqlCommand cmd = new MySqlCommand("Select ID_PRODUK as 'ID PRODUK', NAMA_PRODUK as 'NAMA PRODUK', HARGA_PRODUK as 'HARGA PRODUK', SATUAN, JUMLAH_PRODUK AS 'JUMLAH PRODUK', JUMLAH_MINIMUM_PRODUK AS 'JUMLAH MINIMUM PRODUK', GAMBAR_PRODUK AS 'GAMBAR PRODUK' from produk", conn);
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

        private void GetRecords()
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("Select ID_PRODUK as 'ID PRODUK', NAMA_PRODUK as 'NAMA PRODUK', HARGA_PRODUK as 'HARGA PRODUK', SATUAN, JUMLAH_PRODUK AS 'JUMLAH PRODUK', JUMLAH_MINIMUM_PRODUK AS 'JUMLAH MINIMUM PRODUK', GAMBAR_PRODUK AS 'GAMBAR PRODUK' from produk", conn);
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
                    IdProdukText.Text = selected_row["ID PRODUK"].ToString();
                    NamaProdukText.Text = selected_row["NAMA PRODUK"].ToString();
                    HargaProdukText.Text = selected_row["HARGA PRODUK"].ToString();
                    SatuanText.Text = selected_row["SATUAN"].ToString();
                    JumlahProdukText.Text = selected_row["JUMLAH PRODUK"].ToString();
                    JumlahMinimumProdukText.Text = selected_row["JUMLAH MINIMUM PRODUK"].ToString();

                    // Gambar
                    //GambarProduk = selected_row["GAMBAR_PRODUK"];

                    //ImageSourceConverter imgs = new ImageSourceConverter();
                    //GambarProduk.SetValue(Image.SourceProperty, imgs.ConvertFromString(op.FileName.ToString()));
                    //GambarProduk.Source = imageBuffer;
                    //imageBuffer.GetValue = selected_row["GAMBAR_PRODUK"];
                    //(byte)imageBuffer = (GambarProduk)selected_row["Gambar_Produk"];
                    //GambarProduk.GetValue = 
                    //GambarProduk = ByteToImage(GambarProduk);

                    //Image img = byteArrayToImage(GambarProduk);
                    //var imageBuffer = BitmapSourceToByteArray((BitmapSource)GambarProduk.Source);
                    //GambarProduk = selected_row["GAMBAR_PRODUK", imageBuffer];

                    //byte[] image = (byte[])cmd.ExecuteScalar();
                    //Image newImage = byteArrayToImage(image);
                    //GambarProduk = newImage.Save("GAMBAR_PRODUK");

                    byte[] blob = (byte[])selected_row["Gambar Produk"];
                    MemoryStream ms = new MemoryStream();
                    ms.Write(blob, 0, blob.Length);
                    ms.Position = 0;
                    //byteArrayToImage(blob);
                    //GambarProduk.Source = blob;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private byte[] BitmapSourceToByteArray(BitmapSource image)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder(); // or some other encoder
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.Write(blob, 0, blob.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                Bitmap bm = new Bitmap(mStream);
                return bm;
            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                //op.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
                op.Multiselect = false;
                op.Title = "Select a picture";
                op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                  "Portable Network Graphic (*.png)|*.png";
                if (op.ShowDialog() == true)
                {
                    GambarProduk.Source = new BitmapImage(new Uri(op.FileName));
                    string lokasiGambar = op.FileName.ToString();
                    LokasiGambarText.Text = lokasiGambar;
                }
            }
            catch (Exception brw)
            {
                MessageBox.Show(brw.Message.ToString());
            }
        }

        private void ClearData()
        {
            NamaProdukText.Clear();
            IdProdukText.Clear();
            HargaProdukText.Clear();
            SatuanText.Clear();
            JumlahMinimumProdukText.Clear();
            JumlahProdukText.Clear();
            GambarProduk.Source = null;
            LokasiGambarText.Clear();
        }    
    }
}
