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
using TradeUnionBureauSystem.Model;
using TradeUnionBureauSystem.Views.Pages;

namespace TradeUnionBureauSystem.Views
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        private Users _currentUser;
        public UserWindow(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UserFrame.Navigate(new UserProfilePage(_currentUser));
        }

        private void MembersProfunion_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.Navigate(new MembersProfunionPage(_currentUser));
        }

        private void EventsListButton_Click(object sender, RoutedEventArgs e)
        {
            UserFrame.Navigate(new EventsListPage(_currentUser));
        }
    }
}
