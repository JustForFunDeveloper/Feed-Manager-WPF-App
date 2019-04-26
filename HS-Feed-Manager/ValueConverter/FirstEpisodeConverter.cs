using HS_Feed_Manager.Core.Handler;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HS_Feed_Manager.ValueConverter
{
    public class FirstEpisodeConverter : IValueConverter
    {
        private const double Tolerance = double.Epsilon;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return false;
                var number = (double)value;
                return Math.Abs(number - 1) < Tolerance;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Convert: " + ex.ToString(), LogLevel.Error);
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}