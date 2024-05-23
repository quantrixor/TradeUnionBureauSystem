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
    /// Логика взаимодействия для EventsListPage.xaml
    /// </summary>
    public partial class EventsListPage : Page
    {
        private Users _currentUser;
        public ObservableCollection<Events> PastEvents { get; set; }
        public ObservableCollection<Events> UpcomingEvents { get; set; }
        public ICommand DeleteEventCommand { get; }
        public ICommand AddEventCommand { get; }

        public EventsListPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;

            DeleteEventCommand = new RelayCommand<Events>(DeleteEvent);
            AddEventCommand = new RelayCommand(AddEvent);


            CheckUserRole();
        }

        private void LoadEvents()
        {
            using (var context = new dbProfunionEntities())
            {
                var pastEvents = context.Events.Where(e => e.DateOfEvent < DateTime.Now).ToList();
                var upcomingEvents = context.Events.Where(e => e.DateOfEvent >= DateTime.Now).ToList();

                PastEvents = new ObservableCollection<Events>(pastEvents);
                UpcomingEvents = new ObservableCollection<Events>(upcomingEvents);

                PastEventsListView.ItemsSource = PastEvents;
                UpcomingEventsListView.ItemsSource = UpcomingEvents;
            }
        }

        private void CheckUserRole()
        {
            using (var context = new dbProfunionEntities())
            {
                var allowedRoles = new string[]
                {
                    "Председатель",
                    "Заместитель председателя",
                    "Руководитель информационной комиссии"
                };

                var userRoles = from ur in context.UserRoles
                                join r in context.Roles on ur.RoleID equals r.RoleID
                                where ur.UserID == _currentUser.UserID && allowedRoles.Contains(r.RoleName)
                                select ur;

                if (userRoles.Any())
                {
                    AddNewEvent.Visibility = Visibility.Visible;
                }
                else
                {
                    AddNewEvent.Visibility = Visibility.Collapsed;
                }
            }
        }

        private bool IsCurrentUserInAllowedRole()
        {
            using (var context = new dbProfunionEntities())
            {
                var allowedRoles = new string[]
                {
                    "Председатель",
                    "Заместитель председателя",
                    "Руководитель информационной комиссии"
                };

                var userRoles = from ur in context.UserRoles
                                join r in context.Roles on ur.RoleID equals r.RoleID
                                where ur.UserID == _currentUser.UserID && allowedRoles.Contains(r.RoleName)
                                select ur;

                return userRoles.Any();
            }
        }

        private void AddEvent(object parameter)
        {
            if (IsCurrentUserInAllowedRole())
            {
                // Переход на страницу добавления нового мероприятия
                this.NavigationService.Navigate(new EventCardPage(_currentUser));
            }
            else
            {
                MessageBox.Show("У вас нет прав на добавление мероприятия.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteEvent(Events eventToDelete)
        {
            if (eventToDelete != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить {eventToDelete.Title}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var context = new dbProfunionEntities())
                    {
                        var eventEntity = context.Events.FirstOrDefault(e => e.EventID == eventToDelete.EventID);
                        if (eventEntity != null)
                        {
                            context.Events.Remove(eventEntity);
                            context.SaveChanges();

                            LoadEvents();
                            MessageBox.Show("Мероприятие успешно удалено.", "Удаление завершено", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти мероприятие в базе данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void EventsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = (Events)((ListView)sender).SelectedItem;

            if (selectedEvent != null)
            {
                // Переход на страницу с подробной информацией
                this.NavigationService.Navigate(new EventCardPage(_currentUser, selectedEvent));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public RelayCommand(Action<T> execute) : this(execute, null) { }

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

        private class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute) : this(execute, null) { }

            public RelayCommand(Action<object> execute, Predicate<object> canExecute)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter)
            {
                return _canExecute == null || _canExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }

        private void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new EventCardPage(_currentUser, new Events()));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEvents();
        }
    }
}
