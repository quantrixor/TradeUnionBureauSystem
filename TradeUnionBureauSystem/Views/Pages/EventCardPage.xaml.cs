using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using TradeUnionBureauSystem.Model;

namespace TradeUnionBureauSystem.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для EventCardPage.xaml
    /// </summary>
    public partial class EventCardPage : Page
    {
        private Events _currentEvent;
        public EventCardPage(Events selectedEvent)
        {
            InitializeComponent();
            _currentEvent = selectedEvent;

            LoadEventDetails();
        }
        private void LoadEventDetails()
        {
            using (var context = new dbProfunionEntities())
            {
                var eventDetails = context.Events.FirstOrDefault(e => e.EventID == _currentEvent.EventID);
                if (eventDetails != null)
                {
                    TitleTextBlock.Text = eventDetails.Title;
                    DateTextBlock.Text = $"Дата проведения: {eventDetails.DateOfEvent:d}";
                    InfoTextBlock.Text = "Информация о мероприятии: ..."; // Добавьте дополнительные данные здесь

                    if (eventDetails.PicEvent != null && eventDetails.PicEvent.Length > 0)
                    {
                        var bitmap = new BitmapImage();
                        using (var stream = new System.IO.MemoryStream(eventDetails.PicEvent))
                        {
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                        }
                        EventImage.Source = bitmap;
                    }
                }
            }
        }

        private void EditEvent_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
