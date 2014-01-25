using System; 
using System.Globalization;
using System.Windows; 
using System.Windows.Data;

namespace Player.Services.Converters
{
    public class PlaylistEntryLeftPartWidthConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = (double) values[0] - (double) values[1]; 
            return new GridLength(result);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}