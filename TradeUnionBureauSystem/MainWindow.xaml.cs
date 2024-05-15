using System.Windows;
using TradeUnionBureauSystem.Views.Pages;

namespace TradeUnionBureauSystem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StartPage());
        }
    }
}
