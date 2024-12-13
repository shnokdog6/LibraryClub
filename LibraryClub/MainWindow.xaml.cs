using LibraryClub.Pages;
using System.Windows;

namespace LibraryClub
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new SignInPage());
        }
    }
}
