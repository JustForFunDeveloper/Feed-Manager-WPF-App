using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HS_Feed_Manager.ValueConverter
{
    public class FirstEpisodeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            var number = (double) value;
            return number == 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}