using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class MemberDetailsPage : Page
    {
        private Members _member;
        private List<Positions> _positions;
        private List<Commissions> _commissions; // Добавим список комиссий

        public MemberDetailsPage(Members member)
        {
            InitializeComponent();
            _member = member;
            LoadPositions();
            LoadCommissions(); // Загрузим комиссии
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

        private void LoadCommissions()
        {
            using (var context = new dbProfunionEntities())
            {
                _commissions = context.Commissions.ToList();
                cbxCommission.ItemsSource = _commissions;
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
                cbxCommission.SelectedValue = _member.CommissionID;

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

        private void BackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
