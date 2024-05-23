using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class ProfilaktoryPage : Page
    {
        private Users _currentUser;

        public ProfilaktoryPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            CheckUserRole();
            SelectCurrentMonthTab();
        }

        private void CheckUserRole()
        {
            using (var context = new dbProfunionEntities())
            {
                var allowedRoles = new List<string> { "Председатель", "Заместитель председателя", "Руководитель культурно-массовой комиссии" };
                var userRole = (from ur in context.UserRoles
                                join r in context.Roles on ur.RoleID equals r.RoleID
                                where ur.UserID == _currentUser.UserID && allowedRoles.Contains(r.RoleName)
                                select r.RoleName).FirstOrDefault();

                if (userRole != null)
                {
                    AddButton.Visibility = Visibility.Visible;
                    DeleteButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void LoadCheckInData()
        {
            using (var context = new dbProfunionEntities())
            {
                var data = from c in context.CheckIn
                           join a in context.Accommodation on c.AccommodationID equals a.AccommodationID
                           select new CheckInInfo
                           {
                               Number = c.CheckInID,
                               FullName = c.LastName + " " + c.FirstName + " " + c.MiddleName,
                               StartCheckIn = (DateTime)c.StartCheckIn,
                               EndCheckIn = (DateTime)c.EndCheckIn,
                               Accommodation = a.AccommodationType
                           };

                var checkInData = data.ToList();
                Console.WriteLine($"Total records: {checkInData.Count}");
                JanuaryDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 1).ToList();
                FebruaryDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 2).ToList();
                MarchDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 3).ToList();
                AprilDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 4).ToList();
                MayDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 5).ToList();
                JuneDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 6).ToList();
                JulyDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 7).ToList();
                AugustDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 8).ToList();
                SeptemberDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 9).ToList();
                OctoberDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 10).ToList();
                NovemberDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 11).ToList();
                DecemberDataGrid.ItemsSource = checkInData.Where(c => c.StartCheckIn.Month == 12).ToList();
            }
        }

        private void SelectCurrentMonthTab()
        {
            var currentMonth = DateTime.Now.Month;
            MonthTabControl.SelectedIndex = currentMonth - 1;
        }

        private void MonthTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthTabControl.SelectedItem is TabItem selectedTab)
            {
                List<CheckInInfo> currentData = null;
                switch (selectedTab.Name)
                {
                    case "JanuaryTab":
                        currentData = JanuaryDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "FebruaryTab":
                        currentData = FebruaryDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "MarchTab":
                        currentData = MarchDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "AprilTab":
                        currentData = AprilDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "MayTab":
                        currentData = MayDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "JuneTab":
                        currentData = JuneDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "JulyTab":
                        currentData = JulyDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "AugustTab":
                        currentData = AugustDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "SeptemberTab":
                        currentData = SeptemberDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "OctoberTab":
                        currentData = OctoberDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "NovemberTab":
                        currentData = NovemberDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                    case "DecemberTab":
                        currentData = DecemberDataGrid.ItemsSource as List<CheckInInfo>;
                        break;
                }
                UpdateTotalCount(currentData);
            }
        }
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void UpdateTotalCount(IEnumerable<CheckInInfo> data)
        {
            if (data != null)
            {
                int totalCount = data.Count();
                TotalSumTextBlock.Text = $"Количество оздоровившихся студентов: {totalCount}";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MonthTabControl.SelectedItem as TabItem;
            if (selectedTab != null)
            {
                var monthName = selectedTab.Header.ToString();
                var monthId = GetMonthId(monthName);
                NavigationService.Navigate(new AddCheckInPage(_currentUser, monthId));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MonthTabControl.SelectedItem as TabItem;
            if (selectedTab != null)
            {
                var dataGrid = selectedTab.Content as DataGrid;
                if (dataGrid != null && dataGrid.SelectedItem is CheckInInfo selectedCheckIn)
                {
                    using (var context = new dbProfunionEntities())
                    {
                        var checkIn = context.CheckIn.FirstOrDefault(c => c.CheckInID == selectedCheckIn.Number);
                        if (checkIn != null)
                        {
                            context.CheckIn.Remove(checkIn);
                            context.SaveChanges();
                            LoadCheckInData();
                        }
                    }
                }
            }
        }

        private int GetMonthId(string monthName)
        {
            switch (monthName)
            {
                case "Январь": return 1;
                case "Февраль": return 2;
                case "Март": return 3;
                case "Апрель": return 4;
                case "Май": return 5;
                case "Июнь": return 6;
                case "Июль": return 7;
                case "Август": return 8;
                case "Сентябрь": return 9;
                case "Октябрь": return 10;
                case "Ноябрь": return 11;
                case "Декабрь": return 12;
                default: return 0;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCheckInData();
        }
    }

    public class CheckInInfo
    {
        public int Number { get; set; }
        public string FullName { get; set; }
        public DateTime StartCheckIn { get; set; }
        public DateTime EndCheckIn { get; set; }
        public string Accommodation { get; set; }
    }
}
