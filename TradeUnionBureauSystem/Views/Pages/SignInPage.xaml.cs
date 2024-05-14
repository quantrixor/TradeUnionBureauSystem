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
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для SignInPage.xaml
    /// </summary>
    public partial class SignInPage : Page
    {
        public SignInPage()
        {
            InitializeComponent();
        }
        private void CloseMainWindow()
        {
            var mainWindow = Window.GetWindow(this);

            if (mainWindow != null && mainWindow is MainWindow)
            {
                mainWindow.Close();
            }
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginTextBox.Text;
            string password = PasswordTextBox.Text;

            using (var context = new dbProfunionEntities())
            {
                var user = context.Users.SingleOrDefault(u => u.Username == username && password == u.PasswordHash);
                if (user != null)
                {
                    UserWindow userWindow = new UserWindow(user);
                    userWindow.Show();
                    CloseMainWindow();
                }
                else
                {
                    // Неправильное имя пользователя или пароль
                    MessageBox.Show("Неправильное имя пользователя или пароль.", "Сбой авторизации!",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
}
