﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для RegistrationMemberPage.xaml
    /// </summary>
    public partial class RegistrationMemberPage : Page
    {

        private List<Commissions> _commissions;
        private List<Positions> _positions;
        private byte[] _selectedPhoto;

        public RegistrationMemberPage()
        {
            InitializeComponent();
            LoadPositions();
            LoadCommissions();
        }
        private void LoadCommissions()
        {
            using (var context = new dbProfunionEntities())
            {
                _commissions = context.Commissions.ToList();
                cbxCommission.ItemsSource = _commissions;
            }
        }
        private void LoadPositions()
        {
            using (var context = new dbProfunionEntities())
            {
                _positions = context.Positions.ToList();
                cbxPosition.ItemsSource = _positions;
            }
        }
        private void RegistrationMember_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new dbProfunionEntities())
            {
                // Разделение ФИО на три части
                var fullNameParts = txbFullName.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                string lastName = fullNameParts.Length > 0 ? fullNameParts[0] : string.Empty;
                string firstName = fullNameParts.Length > 1 ? fullNameParts[1] : string.Empty;
                string middleName = fullNameParts.Length > 2 ? fullNameParts[2] : string.Empty;

                // Создание нового пользователя
                var newUser = new Users
                {
                    Username = txbLogin.Text,
                    Email = txbLogin.Text,
                    PasswordHash = txbPassword.Text, // Хеширование пароля
                    PhoneNumber = txbPhoneNumber.Text,
                };
                context.Users.Add(newUser);
                context.SaveChanges();

                // Создание нового члена профсоюза
                var member = new Members
                {
                    LastName = lastName,
                    FirstName = firstName,
                    MiddleName = middleName,
                    PositionID = (int?)cbxPosition.SelectedValue,
                    CommissionID = (int?)cbxCommission.SelectedValue,
                    AcademicGroup = txbAcademicGroup.Text,
                    EntryDate = DateTime.TryParse(txbReceiptDate.Text, out var date) ? date : DateTime.Now,
                    PhoneNumber = txbPhoneNumber.Text,
                    VKLink = txbVkLink.Text,
                    UserID = newUser.UserID,
                    Photo = _selectedPhoto
                };
                context.Members.Add(member);

                // Назначение роли новому пользователю в зависимости от должности
                string selectedPosition = (cbxPosition.SelectedItem as Positions)?.PositionName;
                string selectedCommission = (cbxCommission.SelectedItem as Commissions)?.CommissionName;
                Roles assignedRole = null;

                if (selectedPosition == "Председатель")
                {
                    assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Председатель");
                }
                else if (selectedPosition == "Заместитель председателя")
                {
                    assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Заместитель председателя");
                }
                else if (selectedPosition == "Руководитель")
                {
                    // Назначение роли в зависимости от выбранной комиссии
                    if (selectedCommission != null)
                    {
                        switch (selectedCommission)
                        {
                            case "Культурно-массовая комиссия":
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель культурно-массовой комиссии");
                                break;
                            case "Жилищно-бытовая комиссия":
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель жилищно-бытовой комиссии");
                                break;
                            case "Информационная комиссия":
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель информационной комиссии");
                                break;
                            case "Комиссия общественного контроля":
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель комиссии общественного контроля");
                                break;
                            case "Спортивно-оздоровительная комиссия":
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель спортивно-оздоровительной комиссии");
                                break;
                            default:
                                assignedRole = context.Roles.FirstOrDefault(r => r.RoleName == "Руководитель культурно-массовой комиссии");
                                break;
                        }
                    }
                }

                if (assignedRole != null)
                {
                    var userRole = new UserRoles
                    {
                        UserID = newUser.UserID,
                        RoleID = assignedRole.RoleID
                    };
                    context.UserRoles.Add(userRole);
                }

                context.SaveChanges();

                MessageBox.Show("Новый член профбюро успешно зарегистрирован!", "Регистрация прошла успешно.",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void SelectedImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                _selectedPhoto = System.IO.File.ReadAllBytes(filePath);

                using (var ms = new System.IO.MemoryStream(_selectedPhoto))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = ms;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    UserPicture.ImageSource = image;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
