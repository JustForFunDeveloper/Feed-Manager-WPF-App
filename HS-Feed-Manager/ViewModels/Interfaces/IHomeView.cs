using System;
using System.Collections.Generic;

namespace HS_Feed_Manager.ViewModels.Interfaces
{
    interface IHomeView
    {
        event EventHandler DownloadFeed;
        event EventHandler SearchLocalFolder;
        event EventHandler<List<object>> StartDownloadEpisodes;
        event EventHandler<object> PlayEpisode;
        event EventHandler<object> DeleteEpisode;
        event EventHandler<object> DeleteTvShow;
        event EventHandler<object> SaveEpisodeData;
        event EventHandler<object> SaveTvShowData;
        event EventHandler<object> ToggleAutoDownload;
        event EventHandler<object> OpenFolder;
        void RefreshData();
        void FinishedSearchLocalFolder();
        void UpdateDownloadList(List<object> autoEpisodes);
    }
}
