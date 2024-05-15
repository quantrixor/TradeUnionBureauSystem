using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage : Page
    {
        private Users _currentUser;
        private bool isEditing = false;
        private List<Positions> _positions;
        public UserProfilePage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadUserData();
            LoadPositions();
        }

        private void EditData_Click(object sender, RoutedEventArgs e)
        {
            if (!isEditing)
            {
                // Включить режим редактирования
                txbFullName.IsReadOnly = false;
                cbxPosition.IsEnabled = true;
                txbAcademicGroup.IsReadOnly = false;
                txbReceiptDate.IsReadOnly = false;
                txbPhoneNumber.IsReadOnly = false;
                txbVkLink.IsReadOnly = false;
                ChangeImageButton.Visibility = Visibility.Visible;

                EditData.Content = "Сохранить";
                isEditing = true;
            }
            else
            {
                // Сохранить изменения и выключить режим редактирования
                var member = _currentUser.Members.FirstOrDefault();

                if (member != null)
                {
                    var fullName = txbFullName.Text.Split(' ');

                    member.LastName = fullName.Length > 0 ? fullName[0] : member.LastName;
                    member.FirstName = fullName.Length > 1 ? fullName[1] : member.FirstName;
                    member.MiddleName = fullName.Length > 2 ? fullName[2] : member.MiddleName;
                    member.PositionID = (int?)cbxPosition.SelectedValue;
                    member.AcademicGroup = txbAcademicGroup.Text;
                    member.EntryDate = DateTime.TryParse(txbReceiptDate.Text, out var date) ? date : member.EntryDate;
                    member.PhoneNumber = txbPhoneNumber.Text;
                    member.VKLink = txbVkLink.Text;

                    using (var context = new dbProfunionEntities())
                    {
                        context.Entry(member).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }

                txbFullName.IsReadOnly = true;
                cbxPosition.IsEnabled = false;
                txbAcademicGroup.IsReadOnly = true;
                txbReceiptDate.IsReadOnly = true;
                txbPhoneNumber.IsReadOnly = true;
                txbVkLink.IsReadOnly = true;
                ChangeImageButton.Visibility = Visibility.Collapsed;
                EditData.Content = "Редактировать";
                isEditing = false;
            }
        }

        private void LoadUserData()
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Пользователь не инициализирован.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var member = _currentUser.Members.FirstOrDefault(); // Получение первого элемента из коллекции Members

            if (member != null)
            {
                txbFullName.Text = $"{member.LastName} {member.FirstName} {member.MiddleName}";
                cbxPosition.SelectedValue = member.PositionID;
                txbAcademicGroup.Text = member.AcademicGroup;
                txbReceiptDate.Text = member.EntryDate?.ToString("d") ?? string.Empty;
                txbPhoneNumber.Text = member.PhoneNumber;
                txbVkLink.Text = member.VKLink;

                // Загрузка изображения, если оно существует
                if (member.Photo != null)
                {
                    using (var ms = new System.IO.MemoryStream(member.Photo))
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
        }

        private void LoadPositions()
        {
            using (var context = new dbProfunionEntities())
            {
                _positions = context.Positions.ToList();
                cbxPosition.ItemsSource = _positions;
            }
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var member = _currentUser.Members.FirstOrDefault();

                if (member != null)
                {
                    member.Photo = System.IO.File.ReadAllBytes(filePath);
                    using (var context = new dbProfunionEntities())
                    {
                        context.Entry(member).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }

                    using (var ms = new System.IO.MemoryStream(member.Photo))
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
        }
    }
}
