using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MemberDetailsPage.xaml
    /// </summary>
    public partial class MemberDetailsPage : Page
    {
        private Members _member;
        private List<Positions> _positions;

        public MemberDetailsPage(Members member)
        {
            InitializeComponent();
            _member = member;
            LoadPositions();
            LoadMemberData();
        }
        private void LoadPositions()
        {
            using (var context = new dbProfunionEntities())
            {
                _positions = context.Positions.ToList();
                cbxPosition.ItemsSource = _positions;
            }
        }
        private void LoadMemberData()
        {
            if (_member != null)
            {
                txbFullName.Text = $"{_member.LastName} {_member.FirstName} {_member.MiddleName}";
                cbxPosition.SelectedValue = _member.PositionID;
                txbAcademicGroup.Text = _member.AcademicGroup;
                txbReceiptDate.Text = _member.EntryDate?.ToString("d") ?? string.Empty;
                txbPhoneNumber.Text = _member.PhoneNumber;
                txbVkLink.Text = _member.VKLink;

                // Загрузка изображения, если оно существует
                if (_member.Photo != null)
                {
                    using (var ms = new MemoryStream(_member.Photo))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                        UserPicture.ImageSource = image;
                    }
                }
            }
        }
    }
}
