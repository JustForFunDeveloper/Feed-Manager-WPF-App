using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HS_Feed_Manager.Control;
using HS_Feed_Manager.Core.GlobalValues;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.DataModels.XmlModels;

namespace HS_Feed_Manager.Core
{
    public class Logic
    {
        // TODO: Better Logging
        // TODO: Add File Logging
        #region Public Properties

        public static List<Episode> FeedEpisodes = new List<Episode>();

        public static List<TvShow> LocalTvShows => DbHandler.LocalTvShows;

        public static Config LocalConfig => _config;

        public static string Log => _log;
        #endregion

        #region Private Properties

        private DbHandler _dbHandler;
        private Controller _controller;
        private FeedHandler _feedHandler;
        private FileHandler _fileHandler;
        private FileNameParser _fileNameParser;
        private static Config _config;
        private static string _log;

        #endregion

        public Logic()
        {
            _dbHandler = new DbHandler();
            _controller = new Controller();
            _fileNameParser = new FileNameParser();
            _feedHandler = new FeedHandler(_fileNameParser);
            _fileHandler = new FileHandler(_fileNameParser);
            var unused = new LogHandler(LogLevel.Debug, _fileHandler);

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

            _controller.SaveConfig += OnSaveConfig;
            _controller.RestoreLocalPathSettings += OnRestoreLocalPathSettings;
            _controller.RestoreFeedLinkSettings += OnRestoreFeedLinkSettings;
            _controller.LogRefresh += OnLogRefresh;

            LoadOrCreateConfig();
            OnDownloadFeed(null, null);
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
                    _fileHandler.OpenStandardProgram(episode.Link, false);
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
                _fileHandler.OpenStandardProgram(episode.LocalPath, true);
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

        private void LoadOrCreateConfig()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(LogicConstants.StandardXmlPath + LogicConstants.StandardXmlName);

                if (!fileInfo.Exists)
                {
                    // Create new xml file since its missing and set it to standard values
                    string standardXml = XmlHandler.GetSerializedConfigXml(typeof(Config), new Config());
                    _fileHandler.CreateFileIfNotExist(LogicConstants.StandardXmlName, LogicConstants.StandardXmlPath, false);
                    _fileHandler.AppendText(LogicConstants.StandardXmlName, standardXml, LogicConstants.StandardXmlPath);
                }
                var configAsString = _fileHandler.ReadAllText(LogicConstants.StandardXmlName, LogicConstants.StandardXmlPath);
                _config = (Config)XmlHandler.GetDeserializedConfigObject(typeof(Config), configAsString);
                RefreshLocalConfig();
            }
            catch (Exception e)
            {
                LogHandler.WriteSystemLog(e.ToString(), LogLevel.Error);
            }
        }

        private void RefreshLocalConfig()
        {
            _fileHandler.FileEndings = _config.FileEndings?.Split(';').ToList();
            _fileHandler.LocalPath1 = _config.LocalPath1;
            _fileHandler.LocalPath2 = _config.LocalPath2;
            _fileHandler.LocalPath3 = _config.LocalPath3;

            _feedHandler.FeedUrl = _config.FeedUrl;
            _fileNameParser.NameFrontRegex = _config.NameFrontRegex;
            _fileNameParser.NameBackRegex = _config.NameBackRegex;
            _fileNameParser.NumberFrontRegex = _config.NumberFrontRegex;
            _fileNameParser.NumberBackRegex = _config.NumberBackRegex;
            _controller.RefreshSettingsView();
        }

        private void OnSaveConfig(object sender, object e)
        {
            string standardXml = XmlHandler.GetSerializedConfigXml(typeof(Config), _config);
            _fileHandler.OverwriteFile(LogicConstants.StandardXmlName, standardXml, LogicConstants.StandardXmlPath);
            RefreshLocalConfig();
        }

        private void OnRestoreLocalPathSettings(object sender, object e)
        {
            _config.FileEndings = LogicConstants.FileEndings;
            _config.LocalPath1 = LogicConstants.LocalPath1;
            _config.LocalPath2 = LogicConstants.LocalPath2;
            _config.LocalPath3 = LogicConstants.LocalPath3;
            OnSaveConfig(null, null);
            RefreshLocalConfig();
        }

        private void OnRestoreFeedLinkSettings(object sender, object e)
        {
            _config.FeedUrl = LogicConstants.FeedUrl;
            _config.NameFrontRegex = LogicConstants.NameFrontRegex;
            _config.NameBackRegex = LogicConstants.NameBackRegex;
            _config.NumberFrontRegex = LogicConstants.NumberFrontRegex;
            _config.NumberBackRegex = LogicConstants.NumberBackRegex;
            OnSaveConfig(null, null);
            RefreshLocalConfig();
        }

        private void OnLogRefresh(object sender, object e)
        {
            _log = _fileHandler?.ReadAllText(LogHandler.CurrentLogName, LogicConstants.LogFilePath);
            _controller.RefreshSettingsView();
        }
    }
}
