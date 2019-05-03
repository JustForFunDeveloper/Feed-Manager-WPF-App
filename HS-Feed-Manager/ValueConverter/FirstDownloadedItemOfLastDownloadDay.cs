using HS_Feed_Manager.Core.Handler;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using HS_Feed_Manager.Core;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.ViewModels.Handler;

namespace HS_Feed_Manager.ValueConverter
{
    public class FirstDownloadedItemOfLastDownloadDay : IValueConverter
    {
        private bool _isNew;
        private TvShow _tvShow; 

        public FirstDownloadedItemOfLastDownloadDay()
        {
            Mediator.Register(MediatorGlobal.OnRefreshListView, OnRefreshListView);
        }

        private void OnRefreshListView(object obj)
        {
            _isNew = false;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return false;
                var number = (int)value;

                if (null == _tvShow || !_isNew)
                    GetFirstTvShowFromLatestDownloadDate();

                return number == _tvShow.TvShowId;
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

        private void GetFirstTvShowFromLatestDownloadDate()
        {
            try
            {
                TvShow newestDateTimeTvShow = Logic.LocalTvShows[0];
                foreach (TvShow tvShow in Logic.LocalTvShows)
                {
                    if (tvShow.LatestDownload > newestDateTimeTvShow.LatestDownload)
                        newestDateTimeTvShow = tvShow;
                }
                foreach (TvShow tvShow in Logic.LocalTvShows)
                {
                    if (tvShow.LatestDownload.Date.Equals(newestDateTimeTvShow.LatestDownload.Date))
                    {
                        if (tvShow.LatestDownload < newestDateTimeTvShow.LatestDownload)
                            newestDateTimeTvShow = tvShow;
                    }
                }

                _tvShow = newestDateTimeTvShow;
                _isNew = true;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetFirstTvShowFromLatestDownloadDate: " + ex, LogLevel.Error);
                _tvShow = null;
            }
        }
    }
}
