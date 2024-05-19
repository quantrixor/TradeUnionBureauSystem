using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class MaterialHelpPage : Page
    {
        private Users _currentUser;

        public MaterialHelpPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadAssistanceData();
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

        private void LoadAssistanceData()
        {
            using (var context = new dbProfunionEntities())
            {
                var data = from a in context.Assistance
                           join m in context.Months on a.MonthID equals m.MonthID
                           join p in context.PaymentCriteria on a.PaymentCriteriaID equals p.PaymentCriteriaID
                           select new AssistanceInfo
                           {
                               Number = a.AssistanceID,
                               FullName = a.LastName + " " + a.FirstName + " " + a.MiddleName,
                               PaymentCriteria = p.Criteria,
                               Summa = (decimal)a.Summa,
                               ProtocolNumber = (int)a.ExtractFromProtocol,
                               MonthName = m.MonthName
                           };

                var assistanceData = data.ToList();

                JanuaryDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Январь").ToList();
                FebruaryDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Февраль").ToList();
                MarchDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Март").ToList();
                AprilDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Апрель").ToList();
                MayDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Май").ToList();
                JuneDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Июнь").ToList();
                JuleDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Июль").ToList();
                AugustDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Август").ToList();
                SeptemberDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Сентябрь").ToList();
                OctoberDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Октябрь").ToList();
                NovemberDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Ноябрь").ToList();
                DecemberDataGrid.ItemsSource = assistanceData.Where(a => a.MonthName == "Декабрь").ToList();
            }
        }

        private void SelectCurrentMonthTab()
        {
            var currentMonth = DateTime.Now.Month;

            // Индекс вкладки начинается с 0, поэтому вычитаем 1 из номера текущего месяца
            MonthTabControl.SelectedIndex = currentMonth - 1;
        }

        private void MonthTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MonthTabControl.SelectedItem is TabItem selectedTab)
            {
                List<AssistanceInfo> currentData = null;
                switch (selectedTab.Name)
                {
                    case "JanuaryTab":
                        currentData = JanuaryDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "FebruaryTab":
                        currentData = FebruaryDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "MarchTab":
                        currentData = MarchDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "AprilTab":
                        currentData = AprilDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "MayTab":
                        currentData = MayDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "JuneTab":
                        currentData = JuneDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "JulyTab":
                        currentData = JuleDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "AugustTab":
                        currentData = AugustDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "SeptemberTab":
                        currentData = SeptemberDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "OctoberTab":
                        currentData = OctoberDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "NovemberTab":
                        currentData = NovemberDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                    case "DecemberTab":
                        currentData = DecemberDataGrid.ItemsSource as List<AssistanceInfo>;
                        break;
                }
                UpdateTotalSum(currentData);
            }
        }

        private void UpdateTotalSum(IEnumerable<AssistanceInfo> data)
        {
            if (data != null)
            {
                decimal totalSum = data.Sum(item => item.Summa);
                TotalSumTextBlock.Text = $"Общая сумма выплат за месяц: {totalSum} рублей";
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MonthTabControl.SelectedItem as TabItem;
            if (selectedTab != null)
            {
                var monthName = selectedTab.Header.ToString();
                var monthId = GetMonthId(monthName);
                NavigationService.Navigate(new AddAssistancePage(monthId));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTab = MonthTabControl.SelectedItem as TabItem;
            if (selectedTab != null)
            {
                var dataGrid = selectedTab.Content as DataGrid;
                if (dataGrid != null && dataGrid.SelectedItem is AssistanceInfo selectedAssistance)
                {
                    using (var context = new dbProfunionEntities())
                    {
                        var assistance = context.Assistance.FirstOrDefault(a => a.AssistanceID == selectedAssistance.Number);
                        if (assistance != null)
                        {
                            context.Assistance.Remove(assistance);
                            context.SaveChanges();
                            LoadAssistanceData();
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
    }

    public class AssistanceInfo
    {
        public int Number { get; set; }
        public string FullName { get; set; }
        public string PaymentCriteria { get; set; }
        public decimal Summa { get; set; }
        public int ProtocolNumber { get; set; }
        public string MonthName { get; set; }
    }
}
