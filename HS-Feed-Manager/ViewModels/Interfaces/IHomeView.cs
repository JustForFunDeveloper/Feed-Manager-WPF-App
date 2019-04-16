using System;
using System.Collections.Generic;

namespace HS_Feed_Manager.ViewModels.Interfaces
{
    interface IHomeView
    {
        event EventHandler DownloadFeed;
        event EventHandler SearchLocalFolder;
        event EventHandler<List<object>> StartDownloadEpisodes;
        event EventHandler PlayEpisode;
        event EventHandler DeleteEpisode;
        event EventHandler DeleteSeries;
        event EventHandler<object> SaveEpisodeData;
        event EventHandler<object> SaveLocalSeriesData;
        void RefreshData();
    }
}
