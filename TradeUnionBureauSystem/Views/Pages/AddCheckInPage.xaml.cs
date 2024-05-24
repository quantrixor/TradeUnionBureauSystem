using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class AddCheckInPage : Page
    {
        private Users _currentUser;
        private int _monthId;

        public AddCheckInPage(Users currentUser, int monthId)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _monthId = monthId;
            LoadAccommodationOptions();
        }

        private void LoadAccommodationOptions()
        {
            using (var context = new dbProfunionEntities())
            {
                var accommodations = context.Accommodation.ToList();
                AccommodationComboBox.ItemsSource = accommodations;
                AccommodationComboBox.DisplayMemberPath = "AccommodationType";
                AccommodationComboBox.SelectedValuePath = "AccommodationID";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var lastName = LastNameTextBox.Text;
            var firstName = FirstNameTextBox.Text;
            var middleName = MiddleNameTextBox.Text;
            var startCheckIn = StartCheckInDatePicker.SelectedDate;
            var endCheckIn = EndCheckInDatePicker.SelectedDate;
            var accommodationId = (int?)AccommodationComboBox.SelectedValue;

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(middleName) || !startCheckIn.HasValue || !endCheckIn.HasValue || !accommodationId.HasValue)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new dbProfunionEntities())
            {
                var newCheckIn = new CheckIn
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    StartCheckIn = startCheckIn.Value,
                    EndCheckIn = endCheckIn.Value,
                    AccommodationID = accommodationId.Value
                };

                context.CheckIn.Add(newCheckIn);
                context.SaveChanges();
                MessageBox.Show("Запись о заезде успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
