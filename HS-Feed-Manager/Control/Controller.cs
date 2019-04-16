using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HS_Feed_Manager.ViewModels.Handler;
using HS_Feed_Manager.ViewModels.Interfaces;

namespace HS_Feed_Manager.Control
{
    public class Controller : IHomeView
    {
        #region Events

        public event EventHandler DownloadFeed;
        public event EventHandler SearchLocalFolder;
        public event EventHandler<List<object>> StartDownloadEpisodes;
        public event EventHandler PlayEpisode;
        public event EventHandler DeleteEpisode;
        public event EventHandler DeleteSeries;
        public event EventHandler<object> SaveEpisodeData;
        public event EventHandler<object> SaveLocalSeriesData;

        #endregion

        public void RefreshData()
        {
            Mediator.Register(MediatorGlobal.DownloadFeed, OnDownloadFeed);
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
            PlayEpisode?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeleteEpisode(object e)
        {
            DeleteEpisode?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeleteSeries(object e)
        {
            DeleteSeries?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnSaveEpisodeData(object e)
        {
            SaveEpisodeData?.Invoke(this, e);
        }

        #endregion
    }
}
