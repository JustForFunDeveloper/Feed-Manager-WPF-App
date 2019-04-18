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
    public class Controller : IHomeView
    {
        // TODO: I should probably remove the generic objects with the real classNames.
        //      It's probably better to see right away what sort of object will come from this event.
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

        #endregion
    }
}
