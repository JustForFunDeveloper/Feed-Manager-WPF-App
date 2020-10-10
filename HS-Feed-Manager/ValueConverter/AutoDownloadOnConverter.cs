using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.ValueConverter
{
    public class AutoDownloadOnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return false;
                var name = (string) value;

                TvShow tvShow = Logic.LocalTvShows.Single(show => show.Name.Equals(name));
                if (tvShow == null)
                    return false;

                if (tvShow.AutoDownloadStatus == AutoDownload.On)
                    return true;

                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("Convert: " + ex, LogLevel.Error);
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
