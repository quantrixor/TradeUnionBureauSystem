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
        public ObservableCollection<Members> Members { get; set; }
        public MembersProfunionPage()
        {
            InitializeComponent();
            LoadMembers();
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
    }
}
