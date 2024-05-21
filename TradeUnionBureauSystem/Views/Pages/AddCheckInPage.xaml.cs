﻿using System;
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
            CheckUserRole();
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

        private void CheckUserRole()
        {
            using (var context = new dbProfunionEntities())
            {
                var allowedRoles = new List<string> { "Председатель", "Заместитель председателя", "Руководитель спортивно-оздоровительной комиссии" };
                var userRole = (from ur in context.UserRoles
                                join r in context.Roles on ur.RoleID equals r.RoleID
                                where ur.UserID == _currentUser.UserID && allowedRoles.Contains(r.RoleName)
                                select r.RoleName).FirstOrDefault();

                if (userRole != null)
                {
                    SaveButton.Visibility = Visibility.Visible;
                    this.IsEnabled = false;
                }
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
                NavigationService.GoBack();
            }
        }
    }
}
