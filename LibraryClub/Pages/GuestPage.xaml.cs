using LibraryClub.Windows;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LibraryClub.Pages
{
    /// <summary>
    /// Логика взаимодействия для GuestPage.xaml
    /// </summary>
    public partial class GuestPage : Page
    {
        private readonly LibraryClubEntities m_db = new LibraryClubEntities();

        public GuestPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSignInPage();
        }

        private void NavigateToSignInPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new SignInPage());

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Products.ItemsSource = m_db.Products.ToList();
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedProduct = Products.SelectedItem as Product;
            if (selectedProduct == null) return;
            (new ProductDetailsWindow(selectedProduct)).Show();


        }
    }
}
