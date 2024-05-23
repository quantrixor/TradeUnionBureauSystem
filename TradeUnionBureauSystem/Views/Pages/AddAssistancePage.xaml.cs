using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TradeUnionBureauSystem.Model;
using static System.Net.Mime.MediaTypeNames;

namespace TradeUnionBureauSystem.Views.Pages
{
    public partial class AddAssistancePage : Page
    {
        private int _selectedMonthId;

        public AddAssistancePage(int selectedMonthId)
        {
            InitializeComponent();
            _selectedMonthId = selectedMonthId;
            LoadPaymentCriteria();
        }

        private void LoadPaymentCriteria()
        {
            using (var context = new dbProfunionEntities())
            {
                var criteria = context.PaymentCriteria.ToList();
                PaymentCriteriaComboBox.ItemsSource = criteria;
                PaymentCriteriaComboBox.DisplayMemberPath = "Criteria";
                PaymentCriteriaComboBox.SelectedValuePath = "PaymentCriteriaID";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LastNameTextBox.Text) || string.IsNullOrEmpty(FirstNameTextBox.Text) ||
                string.IsNullOrEmpty(MiddleNameTextBox.Text) || PaymentCriteriaComboBox.SelectedValue == null ||
                string.IsNullOrEmpty(SummaTextBox.Text) || string.IsNullOrEmpty(ProtocolNumberTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            decimal summa;
            if (!decimal.TryParse(SummaTextBox.Text, out summa))
            {
                MessageBox.Show("Пожалуйста, введите корректную сумму.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int protocolNumber;
            if (!int.TryParse(ProtocolNumberTextBox.Text, out protocolNumber))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер протокола.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var context = new dbProfunionEntities())
            {
                var newAssistance = new Assistance
                {
                    LastName = LastNameTextBox.Text,
                    FirstName = FirstNameTextBox.Text,
                    MiddleName = MiddleNameTextBox.Text,
                    PaymentCriteriaID = (int)PaymentCriteriaComboBox.SelectedValue,
                    Summa = summa,
                    ExtractFromProtocol = protocolNumber,
                    MonthID = _selectedMonthId
                };

                context.Assistance.Add(newAssistance);
                context.SaveChanges();
                MessageBox.Show("Материальная помощь успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
               
            }
        }

        private void SummaTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void SummaTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9.,]+"); // Разрешены только цифры и символы запятой/точки
            return !regex.IsMatch(text);
        }

        private void ProtocolNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на предыдущую страницу
            NavigationService.GoBack();
        }
    }
}
