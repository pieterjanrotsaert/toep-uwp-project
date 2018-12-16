using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PrettigLokaal.ViewModels.Helpers
{
    class LineConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string str = (string)value;
            return str.Replace('\n', ' ').Replace('\r', ' ');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
