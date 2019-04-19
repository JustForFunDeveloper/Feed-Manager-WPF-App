using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HS_Feed_Manager.Control;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core
{
    public class Logic
    {
        // TODO: Better Logging
        // TODO: Add File Logging
        #region Public Properties

        public static List<Episode> FeedEpisodes = new List<Episode>();

        public static List<TvShow> LocalTvShows => DbHandler.LocalTvShows;

        #endregion

        #region Private Properties

        private DbHandler _dbHandler;
        private Controller _controller;
        private FeedHandler _feedHandler;
        private FileHandler _fileHandler;

        #endregion

        public Logic()
        {
            _dbHandler = new DbHandler();
            _controller = new Controller();
            var fileNameParser = new FileNameParser();
            _feedHandler = new FeedHandler(fileNameParser);
            _fileHandler = new FileHandler(fileNameParser);

            _controller.DownloadFeed += OnDownloadFeed;
            _controller.SearchLocalFolder += OnSearchLocalFolder;
            _controller.StartDownloadEpisodes += OnStartDownloadEpisodes;
            _controller.PlayEpisode += OnPlayEpisode;
            _controller.DeleteEpisode += OnDeleteEpisode;
            _controller.DeleteTvShow += OnDeleteTvShow;
            _controller.ToggleAutoDownload += OnToggleAutoDownload;
            _controller.SaveEpisodeData += _dbHandler.UpdateEpisode;
            _controller.SaveTvShowData += _dbHandler.UpdateTvShow;
            _controller.OpenFolder += OpenFolder;
        }

        private void OnDownloadFeed(object sender, EventArgs e)
        {
            FeedEpisodes.Clear();
            FeedEpisodes.AddRange(_feedHandler.DownloadFeedList());
            List<object> autoEpisodes = new List<object>();
            foreach (var feedEpisode in FeedEpisodes)
            {
                if (LocalTvShows.Any(tvShow =>
                    tvShow.Name.Equals(feedEpisode.Name)
                    && tvShow.AutoDownloadStatus.Equals(AutoDownload.On)
                    && !tvShow.Episodes.Any(episode => episode.EpisodeNumber.Equals(feedEpisode.EpisodeNumber))))
                    autoEpisodes.Add(feedEpisode);
            }

            _controller.UpdateDownloadList(autoEpisodes);
            _controller.RefreshData();
        }

        private async void OnSearchLocalFolder(object sender, EventArgs e)
        {
            await Task.Run(() => ScanFolder());
            _controller.FinishedSearchLocalFolder();
            _controller.RefreshData();
        }

        private void OnStartDownloadEpisodes(object sender, List<object> e)
        {
            foreach (Episode episode in e)
            {
                if (episode != null)
                {
                    _fileHandler.OpenStandardProgram(episode.Link);
                    episode.DownloadDate = DateTime.Now;
                    if (LocalTvShows.Any(tvShow => tvShow.Name.Equals(episode.Name)))
                    {
                        TvShow localTvShow = LocalTvShows.SingleOrDefault(tvShow => tvShow.Name.Equals(episode.Name));
                        if (localTvShow != null &&
                            localTvShow.Episodes.Any(ep => ep.EpisodeNumber.Equals(episode.EpisodeNumber)))
                        {
                            Episode localEpisode =
                                localTvShow.Episodes.SingleOrDefault(ep =>
                                    ep.EpisodeNumber.Equals(episode.EpisodeNumber));
                            if (localEpisode != null)
                            {
                                localTvShow.LatestDownload = episode.DownloadDate;
                                localEpisode.DownloadDate = episode.DownloadDate;
                                _dbHandler.UpdateEpisode(null,localEpisode);
                            }
                        }
                        else if (localTvShow != null)
                        {
                            localTvShow.LatestDownload = episode.DownloadDate;
                            localTvShow.Episodes.Add(episode);
                            _dbHandler.UpdateTvShow(null, localTvShow);
                        }
                    }
                    else
                    {
                        TvShow newTvShow = new TvShow()
                        {
                            Name = episode.Name,
                            Episodes = new List<Episode>() { episode }
                        };
                        newTvShow.LatestDownload = episode.DownloadDate;
                        _dbHandler.AddTvShow(newTvShow, true);
                    }
                }
                Thread.Sleep(10);
            }
            _controller.RefreshData();
        }

        private void OnPlayEpisode(object sender, object e)
        {
            Episode episode = e as Episode;
            if (episode != null)
                _fileHandler.OpenStandardProgram(episode.LocalPath);
        }

        private void ScanFolder()
        {
            List<TvShow> tvShows = _fileHandler.ScanLocalTvShows();
            _dbHandler.SyncLocalTvShows(tvShows);
        }

        private void OnDeleteEpisode(object sender, object e)
        {
            _fileHandler.DeleteEpisode((Episode) e);
            _dbHandler.DeleteEpisode((Episode) e);
            _controller.RefreshData();
        }

        private void OnDeleteTvShow(object sender, object e)
        {
            _fileHandler.DeleteTvShow((TvShow) e);
            _dbHandler.DeleteTvShow((TvShow) e);
            _controller.RefreshData();
        }

        private void OnToggleAutoDownload(object sender, object e)
        {
            TvShow tvShow = e as TvShow;
            if (tvShow != null && tvShow.TvShowId.Equals(0))
                _dbHandler.AddTvShow(tvShow, true);
            else
                _dbHandler.UpdateTvShow(null, e);
        }

        private void OpenFolder(object sender, object e)
        {
            _fileHandler.OpenFolder((Episode) e);
        }
    }
}
