using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bcypt = BCrypt.Net.BCrypt;

namespace LibraryClub.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        private readonly LibraryClubEntities m_db = new LibraryClubEntities();


        public SignInPage()
        {
            InitializeComponent();
        }


        private void GoToGuestPageBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToGuestPage();
        }

        private void NavigateToGuestPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new GuestPage());

        }

        private void GoToSingUpPageBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSignUpPage();
        }

        private void NavigateToSignUpPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new SignUpPage());

        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Login.Text))
            {
                MessageBox.Show("Введите логин");
                return;
            }

            if (string.IsNullOrEmpty(Password.Password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            var user = m_db.Users
                .Where(temp => temp.Login.Equals(Login.Text))
                .FirstOrDefault();

            if (user == null)
            {
                MessageBox.Show("Пользователь не найден");
                return;
            }

            if (!Bcypt.Verify(Password.Password, user.PasswordHash))
            {
                MessageBox.Show("Неверный пароль");
                return;
            }

            Session.CurrentUser = user;

            NavigateToUserPage();
        }

        private void NavigateToUserPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new UserPage());

        }
    }
}
