using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Player.Services.Converters
{
    // TODO move playlist window when stripe changes its position on screen and playlist is visible
    // unfortunately plays badly with (borderless is the reason?) stickywindow
    public class MoveTopValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ok, this is simple, it only demonstrates what happens
            if (value is double && parameter is Window)
            {
                var top = (double)value;
                var window = (Window)parameter;
                // here i must check on which side the window sticks on
                return top;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public class MoveLeftValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // ok, this is simple, it only demonstrates what happens
            if (value is double && parameter is Window)
            {
                var left = (double)value;
                var window = (Window)parameter;
                // here i must check on which side the window sticks on
                return left;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}