using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class EventCardPage : Page
    {
        private Events _currentEvent;
        private byte[] _eventPhoto;
        private Users _currentUser;
        public EventCardPage(Users currentUser, Events selectedEvent = null)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _currentEvent = selectedEvent ?? new Events();
            LoadEventDetails();

            CheckUserRole();
        }

        private void LoadEventDetails()
        {
            if (_currentEvent.EventID != 0)
            {
                using (var context = new dbProfunionEntities())
                {
                    var eventDetails = context.Events.FirstOrDefault(e => e.EventID == _currentEvent.EventID);
                    if (eventDetails != null)
                    {
                        TitleTextBox.Text = eventDetails.Title;
                        InfoTextBox.Text = eventDetails.Desriptioin; // Дополнительные данные здесь
                        DatePicker.SelectedDate = eventDetails.DateOfEvent;

                        if (eventDetails.PicEvent != null && eventDetails.PicEvent.Length > 0)
                        {
                            var bitmap = new BitmapImage();
                            using (var stream = new System.IO.MemoryStream(eventDetails.PicEvent))
                            {
                                bitmap.BeginInit();
                                bitmap.StreamSource = stream;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                            }
                            EventImage.Source = bitmap;
                            _eventPhoto = eventDetails.PicEvent;
                        }
                    }
                }
            }
        }

        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _eventPhoto = System.IO.File.ReadAllBytes(openFileDialog.FileName);

                var bitmap = new BitmapImage();
                using (var stream = new System.IO.MemoryStream(_eventPhoto))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                }
                EventImage.Source = bitmap;
            }
        }

        private void SaveEvent_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new dbProfunionEntities())
            {
                if (_currentEvent.EventID == 0)
                {
                    // Новый объект, добавляем его в базу данных
                    _currentEvent = new Events();
                    context.Events.Add(_currentEvent);
                }
                else
                {
                    // Обновляем существующий объект
                    _currentEvent = context.Events.FirstOrDefault(ev => ev.EventID == _currentEvent.EventID);
                }

                _currentEvent.Title = TitleTextBox.Text;
                _currentEvent.DateOfEvent = DatePicker.SelectedDate ?? DateTime.Now;
                _currentEvent.Desriptioin = InfoTextBox.Text;
                _currentEvent.PicEvent = _eventPhoto;

                context.SaveChanges();
                MessageBox.Show("Мероприятие сохранено.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void CheckUserRole()
        {
            if (!IsCurrentUserInAllowedRole())
            {
                TitleTextBox.IsReadOnly = true;
                InfoTextBox.IsReadOnly = true;
                DatePicker.IsEnabled = false;
                UploadPhotoButton.IsEnabled = false;
                UploadPhotoButton.Visibility = Visibility.Collapsed;
                SaveEventButton.Visibility = Visibility.Collapsed;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
