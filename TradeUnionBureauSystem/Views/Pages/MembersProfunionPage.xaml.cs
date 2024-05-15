using System.Collections.ObjectModel;
using System.Linq;
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

        private void RegisterNewMember_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
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
