using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для MembersProfunionPage.xaml
    /// </summary>
    public partial class MembersProfunionPage : Page, INotifyPropertyChanged
    {
        private Users _currentUser;
        private bool _isChairman;

        public ObservableCollection<Members> Members { get; set; }

        public bool IsChairman
        {
            get { return _isChairman; }
            set
            {
                _isChairman = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteMemberCommand { get; }

        public MembersProfunionPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;

            DeleteMemberCommand = new RelayCommand<Members>(DeleteMember);

            LoadMembers();
            CheckChairmanRole();
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

        private void CheckChairmanRole()
        {
            IsChairman = IsCurrentUserChairman();
        }

        private bool IsCurrentUserChairman()
        {
            using (var context = new dbProfunionEntities())
            {
                var chairmanRole = context.Roles.FirstOrDefault(r => r.RoleName == "Председатель");
                if (chairmanRole == null)
                    return false;

                var userRole = context.UserRoles.FirstOrDefault(ur => ur.UserID == _currentUser.UserID && ur.RoleID == chairmanRole.RoleID);

                return userRole != null;
            }
        }

        private void RegisterNewMember_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationMemberPage());
        }
        private void DeleteMember(Members member)
        {
            if (member != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить {member.LastName} {member.FirstName} {member.MiddleName}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new dbProfunionEntities())
                    {
                        // Загрузим объект из контекста перед удалением
                        var memberToDelete = context.Members.FirstOrDefault(m => m.MemberID == member.MemberID);

                        if (memberToDelete != null)
                        {
                            var userRoles = context.UserRoles.Where(ur => ur.UserID == memberToDelete.UserID);
                            context.UserRoles.RemoveRange(userRoles);

                            var user = context.Users.FirstOrDefault(u => u.UserID == memberToDelete.UserID);
                            if (user != null)
                            {
                                context.Users.Remove(user);
                            }

                            context.Members.Remove(memberToDelete);
                            context.SaveChanges();

                            LoadMembers();
                            MessageBox.Show("Член профсоюза успешно удален.", "Удаление завершено", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти участника в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите участника для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        public RelayCommand(Action<T> execute) : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
