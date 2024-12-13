using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bcypt = BCrypt.Net.BCrypt;

namespace LibraryClub.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        private readonly LibraryClubEntities m_db = new LibraryClubEntities();
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

            if (user != null)
            {
                MessageBox.Show("Логин занят");
                return;
            }

            var userDto = new User()
            {
                Login = Login.Text,
                PasswordHash = Bcypt.HashPassword(Password.Password),
                RoleId = 3
            };

            user = m_db.Users.Add(userDto);
            m_db.SaveChanges();

            Session.CurrentUser = user;
            NavigateToUserPage();
        }

        private void NavigateToUserPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new UserPage());

        }

        private void NavigateToSignInPage()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.MainFrame.Navigate(new SignInPage());

        }

        private void GoToSignInPageBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSignInPage();
        }
    }
}
