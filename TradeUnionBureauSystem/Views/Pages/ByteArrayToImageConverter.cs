using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TradeUnionBureauSystem.Views.Pages
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is byte[] bytes)
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                }

                // Получаем размеры изображения
                int width = image.PixelWidth;
                int height = image.PixelHeight;

                // Обрезаем изображение по грудь
                int croppedHeight = (int)(height * 0.5); // Обрезаем 50% от высоты изображения, это можно настроить
                CroppedBitmap croppedImage = new CroppedBitmap(image, new Int32Rect(0, 0, width, croppedHeight));

                return croppedImage;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
