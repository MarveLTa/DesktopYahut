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
using System.Windows.Shapes;

namespace KouveePetShop
{
    /// <summary>
    /// Interaction logic for UiDashboard.xaml
    /// </summary>
    public partial class UiDashboard : Window
    {
        public UiDashboard()
        {
            InitializeComponent();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnBellNotif_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAccount_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex;
            MoveCursorMenu(index);

            switch (index)
            {
                case 0:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Visible;
                    BtnUkuranHewan.Visibility = Visibility.Visible;
                    BtnHewan.Visibility = Visibility.Visible;
                    break;
                case 1:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Collapsed;
                    BtnUkuranHewan.Visibility = Visibility.Collapsed;
                    BtnHewan.Visibility = Visibility.Collapsed;
                    GridPrincipal.Children.Add(new CrudJasaLayanan());
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Collapsed;
                    BtnUkuranHewan.Visibility = Visibility.Collapsed;
                    BtnHewan.Visibility = Visibility.Collapsed;
                    GridPrincipal.Children.Add(new CrudProduk());
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Collapsed;
                    BtnUkuranHewan.Visibility = Visibility.Collapsed;
                    BtnHewan.Visibility = Visibility.Collapsed;
                    GridPrincipal.Children.Add(new CrudCustomer());
                    break;
                case 4:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Collapsed;
                    BtnUkuranHewan.Visibility = Visibility.Collapsed;
                    BtnHewan.Visibility = Visibility.Collapsed;
                    GridPrincipal.Children.Add(new CrudPegawai());
                    break;
                case 5:
                    GridPrincipal.Children.Clear();
                    BtnBack.Visibility = Visibility.Collapsed;
                    BtnJenisHewan.Visibility = Visibility.Collapsed;
                    BtnUkuranHewan.Visibility = Visibility.Collapsed;
                    BtnHewan.Visibility = Visibility.Collapsed;
                    GridPrincipal.Children.Add(new CrudSupplier());
                    break;
                case 6:
                    string message = "Apakah anda yakin ingin keluar ?";
                    string caption = "Warning";
                    MessageBoxButton buttons = MessageBoxButton.YesNo;
                    MessageBoxImage icon = MessageBoxImage.Question;

                    if (MessageBox.Show(message, caption, buttons, icon) == MessageBoxResult.Yes)
                    {
                        UiLogin login = new UiLogin();
                        login.Show();
                        this.Close();
                    }
                    break;
                default:
                    break;
            }
        }

        private void MoveCursorMenu(int index)
        {
            TransitioningContentSlide.OnApplyTemplate();
            GridCursor.Margin = new Thickness(0, (100 + (60 * index)), 0, 0);
        }

        private void BtnUkuranHewan_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            BtnBack.Visibility = Visibility.Visible;
            BtnJenisHewan.Visibility = Visibility.Collapsed;
            BtnUkuranHewan.Visibility = Visibility.Collapsed;
            BtnHewan.Visibility = Visibility.Collapsed;
            GridPrincipal.Children.Add(new CrudUkuranHewan());
        }

        private void BtnJenisHewan_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            BtnBack.Visibility = Visibility.Visible;
            BtnJenisHewan.Visibility = Visibility.Collapsed;
            BtnUkuranHewan.Visibility = Visibility.Collapsed;
            BtnHewan.Visibility = Visibility.Collapsed;
            GridPrincipal.Children.Add(new CrudJenisHewan());
        }

        private void BtnHewan_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            BtnBack.Visibility = Visibility.Visible;
            BtnJenisHewan.Visibility = Visibility.Collapsed;
            BtnUkuranHewan.Visibility = Visibility.Collapsed;
            BtnHewan.Visibility = Visibility.Collapsed;
            GridPrincipal.Children.Add(new CrudHewan());
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            GridPrincipal.Children.Clear();
            BtnBack.Visibility = Visibility.Collapsed;
            BtnJenisHewan.Visibility = Visibility.Visible;
            BtnUkuranHewan.Visibility = Visibility.Visible;
            BtnHewan.Visibility = Visibility.Visible;
        }
    }
}
