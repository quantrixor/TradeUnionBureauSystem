using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MembersProfunionPage.xaml
    /// </summary>
    public partial class MembersProfunionPage : Page
    {
        private Users _currentUser;
        public ObservableCollection<Members> Members { get; set; }
        public MembersProfunionPage(Users currentUser)
        {
            InitializeComponent();
            LoadMembers();
            
            _currentUser = currentUser;
            DataContext = this;

            // Отображение кнопки регистрации нового члена профсоюза только для председателя
            RegisterNewMemberButton.Visibility = IsCurrentUserChairman() ? Visibility.Visible : Visibility.Collapsed;

        }
        private void LoadMembers()
        {
            using (var context = new dbProfunionEntities())
            {
                var members = context.Members.ToList();
                Members = new ObservableCollection<Members>(members);
                MembersListView.ItemsSource = Members;
            }
        }
        private bool IsCurrentUserChairman()
        {
            using (var context = new dbProfunionEntities())
            {
                var chairmanRole = context.Roles.FirstOrDefault(r => r.RoleName == "Председатель");
                if (chairmanRole == null)
                    return false;

                var userRole = context.UserRoles.FirstOrDefault(ur => ur.UserID == _currentUser.UserID && ur.RoleID == chairmanRole.RoleID);

                if (userRole != null)
                {
                   // MessageBox.Show("Пользователь является председателем профсоюза.", "Роль подтверждена", MessageBoxButton.OK, MessageBoxImage.Information);
                    return true;
                }
                else
                {
                   // MessageBox.Show("Пользователь не является председателем профсоюза.", "Роль не подтверждена", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
        }

        private void RegisterNewMember_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationMemberPage());
        }

        private void MembersListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedMember = (Members)MembersListView.SelectedItem;

            if (selectedMember != null)
            {
                if (selectedMember.UserID == _currentUser.UserID)
                {
                    // Открыть страницу профиля пользователя с возможностью редактирования
                    this.NavigationService.Navigate(new UserProfilePage(_currentUser));
                }
                else
                {
                    // Открыть страницу просмотра профиля
                    this.NavigationService.Navigate(new MemberDetailsPage(selectedMember));
                }
            }
        }
    }
}
