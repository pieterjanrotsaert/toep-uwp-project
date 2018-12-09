using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace PrettigLokaal.ViewModels.Helpers
{
    class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string base64 = (string)value;
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            byte[] buf = System.Convert.FromBase64String(base64);
            BitmapImage bitmap = new BitmapImage();
            MemoryStream stream = new MemoryStream(buf);
            bitmap.SetSourceAsync(stream.AsRandomAccessStream()); // Don't await this or the UI will block.
            return bitmap;
        }

        // Not necessary
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
