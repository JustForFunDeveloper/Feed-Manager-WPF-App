using System;
using System.Collections.Generic;
using HS_Feed_Manager.ViewModels.Handler;
using HS_Feed_Manager.ViewModels.Interfaces;

namespace HS_Feed_Manager.Control
{
    /// <summary>
    /// This Controller class implements all interfaces from the UI.
    /// This should be the only place where the logic or the core functionality is connected to the UI.
    /// </summary>
    public class Controller : IHomeView, ISettingsView
    {
        #region Events

        public event EventHandler DownloadFeed;
        public event EventHandler SearchLocalFolder;
        public event EventHandler<List<object>> StartDownloadEpisodes;
        public event EventHandler<object> PlayEpisode;
        public event EventHandler<object> DeleteEpisode;
        public event EventHandler<object> DeleteTvShow;
        public event EventHandler<object> SaveEpisodeData;
        public event EventHandler<object> SaveTvShowData;
        public event EventHandler<object> ToggleAutoDownload;
        public event EventHandler<object> OpenFolder;

        public event EventHandler SaveConfig;
        public event EventHandler RestoreLocalPathSettings;
        public event EventHandler RestoreFeedLinkSettings;
        public event EventHandler LogRefresh;

        #endregion

        public Controller()
        {
            Mediator.Register(MediatorGlobal.DownloadFeed, OnDownloadFeed);
            Mediator.Register(MediatorGlobal.SearchLocalFolder, OnSearchLocalFolder);
            Mediator.Register(MediatorGlobal.StartDownloadEpisodes, OnStartDownloadEpisodes);
            Mediator.Register(MediatorGlobal.PlayEpisode, OnPlayEpisode);
            Mediator.Register(MediatorGlobal.DeleteEpisode, OnDeleteEpisode);
            Mediator.Register(MediatorGlobal.DeleteTvShow, OnDeleteTvShow);
            Mediator.Register(MediatorGlobal.SaveEpisodeEditInfo, OnSaveEpisodeData);
            Mediator.Register(MediatorGlobal.SaveEditInfo, OnSaveTvShowData);
            Mediator.Register(MediatorGlobal.ToggleAutoDownload, OnToggleAutoDownload);
            Mediator.Register(MediatorGlobal.OpenFolder, OnOpenFolder);
            Mediator.Register(MediatorGlobal.SaveConfig, OnSaveConfig);
            Mediator.Register(MediatorGlobal.RestoreLocalPathSettings, OnRestoreLocalPathSettings);
            Mediator.Register(MediatorGlobal.RestoreFeedLinkSettings, OnRestoreFeedLinkSettings);
            Mediator.Register(MediatorGlobal.LogRefresh, OnLogRefresh);
        }

        public void RefreshData()
        {
            Mediator.NotifyColleagues(MediatorGlobal.OnRefreshListView, null);
        }

        public void FinishedSearchLocalFolder()
        {
            Mediator.NotifyColleagues(MediatorGlobal.FinishedSearchLocalFolder, null);
        }

        public void UpdateDownloadList(List<object> autoEpisodes)
        {
            Mediator.NotifyColleagues(MediatorGlobal.UpdateDownloadList, autoEpisodes);
        }

        public void RefreshSettingsView()
        {
            Mediator.NotifyColleagues(MediatorGlobal.RefreshSettingsView, null);
        }

        #region Event Invocations

        protected virtual void OnDownloadFeed(object e)
        {
            DownloadFeed?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSearchLocalFolder(object e)
        {
            SearchLocalFolder?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnStartDownloadEpisodes(object e)
        {
            StartDownloadEpisodes?.Invoke(this, (List<object>) e);
        }

        protected virtual void OnPlayEpisode(object e)
        {
            PlayEpisode?.Invoke(this, e);
        }

        protected virtual void OnDeleteEpisode(object e)
        {
            DeleteEpisode?.Invoke(this, e);
        }

        protected virtual void OnDeleteTvShow(object e)
        {
            DeleteTvShow?.Invoke(this, e);
        }

        protected virtual void OnSaveEpisodeData(object e)
        {
            SaveEpisodeData?.Invoke(this, e);
        }

        protected virtual void OnSaveTvShowData(object e)
        {
            SaveTvShowData?.Invoke(this, e);
        }

        protected virtual void OnToggleAutoDownload(object e)
        {
            ToggleAutoDownload?.Invoke(this, e);
        }

        protected virtual void OnOpenFolder(object e)
        {
            OpenFolder?.Invoke(this, e);
        }

        protected virtual void OnSaveConfig(object e)
        {
            SaveConfig?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRestoreLocalPathSettings(object e)
        {
            RestoreLocalPathSettings?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnRestoreFeedLinkSettings(object e)
        {
            RestoreFeedLinkSettings?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnLogRefresh(object e)
        {
            LogRefresh?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
