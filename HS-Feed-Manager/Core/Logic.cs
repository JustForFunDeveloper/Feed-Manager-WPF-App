using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using HS_Feed_Manager.Control;
using HS_Feed_Manager.Core.GlobalValues;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.DataModels.XmlModels;
using HS_Feed_Manager.ViewModels.Handler;
using Serilog;

namespace HS_Feed_Manager.Core
{
    public class Logic
    {
        #region Public Properties

        public static List<Episode> FeedEpisodes = new List<Episode>();

        public static List<TvShow> LocalTvShows => DbHandler.LocalTvShows;

        public static Config LocalConfig => _config;
        public static string SqLiteSqLiteLog => _sqLiteLog;
        
        #endregion

        #region Private Properties

        private DbHandler _dbHandler;
        private Controller _controller;
        private FeedHandler _feedHandler;
        private FileHandler _fileHandler;
        private CancellationTokenSource _copyDownloadTokenSource;
        private Task _copyDownloadTask;
        
        private static Config _config;
        private static string _sqLiteLog;

        #endregion

        public Logic()
        {
            _dbHandler = new DbHandler();
            _controller = new Controller();
            _feedHandler = new FeedHandler();
            _fileHandler = new FileHandler();

            _fileHandler.ExceptionEvent += OnExceptionEvent;

            _controller.DownloadFeed += OnDownloadFeed;
            _controller.SearchLocalFolder += OnSearchLocalFolder;
            _controller.StartDownloadEpisodes += OnStartDownloadEpisodes;
            _controller.CopyFromDownload += OnCopyFromDownload;
            _controller.StopCopyFromDownload += OnStopCopyFromDownload;
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

            try
            {
                LoadOrCreateConfig();
                OnDownloadFeed(null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"Logic Error!");
            }
        }

        private async void OnDownloadFeed(object sender, EventArgs e)
        {
            try
            {
                FeedEpisodes.Clear();
                FeedEpisodes?.AddRange(await _feedHandler.DownloadFeedList());
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
            catch (Exception ex)
            {
                Log.Error(ex,"OnDownloadFeed Error!");
            }
        }

        private async void OnSearchLocalFolder(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() => ScanFolder());
                _controller.FinishedSearchLocalFolder();
                _controller.RefreshData();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnSearchLocalFolder Error!");
            }
        }

        private void OnStartDownloadEpisodes(object sender, List<object> e)
        {
            try
            {
                foreach (Episode episode in e)
                {
                    if (episode != null)
                    {
                        //Download Torrent
                        _fileHandler.OpenStandardProgram(episode.Link, false);
                        
                        // Wait for successful download
                        // Thread.Sleep(1000);
                        //Change into magnet link and start downloading
                        // _fileHandler.OpenStandardProgram(GetMagnetLink(LocalConfig.DownloadFolder + FileNameParser.TorrentNameParser(episode.Link)), false);

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
                                    _dbHandler.UpdateEpisode(null, localEpisode);
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
            catch (Exception ex)
            {
                Log.Error(ex,"OnStartDownloadEpisodes Error!");
            }
        }

        private void OnStopCopyFromDownload(object sender, EventArgs e)
        {
            if (_copyDownloadTask == null) return;
            Log.Information("Stop Copy Downloaded Shows");
            _copyDownloadTokenSource.Cancel();
            _copyDownloadTask = null;
        }

        private void OnCopyFromDownload(object sender, EventArgs e)
        {
            try
            {
                _copyDownloadTokenSource = new CancellationTokenSource();
                CancellationToken ct = _copyDownloadTokenSource.Token;
                _copyDownloadTask = Task.Run(() =>
                {
                    try
                    {
                        // Get and copy shows from DownloadFolder to the CopyToFolder
                        var tvShows = _fileHandler.GetAndCopyDownloadedShows(ct);
                    
                        // Sync to database
                        if (tvShows != null)
                        {
                            _dbHandler.SyncLocalTvShows(tvShows);
                            var sum = tvShows?.Sum(tv => tv.Episodes.Count);
                            Log.Information($"Finished Copy {sum} Downloaded Shows");
                        }
                        else
                        {
                            Log.Information("Nothing to copy. Folder is empty.");
                        }
                        Mediator.NotifyColleagues(MediatorGlobal.FinishedCopyDownload, null);
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception,"Task OnCopyFromDownload Error!");
                        Mediator.NotifyColleagues(MediatorGlobal.FinishedCopyDownload, null);
                    }
                }, ct);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnCopyFromDownload Error!");
            }
        }

        private string GetMagnetLink(string path)
        {
            var parser = new BencodeParser();
            Torrent torrent = parser.Parse<Torrent>(path);

            // Calculate the info hash
            //string infoHash = torrent.GetInfoHash();
            //byte[] infoHashBytes = torrent.GetInfoHashBytes();
            string magnetLink = torrent.GetMagnetLink();
            return magnetLink;
        }

        private void OnPlayEpisode(object sender, object e)
        {
            try
            {
                Episode episode = e as Episode;
                if (episode != null)
                    _fileHandler.OpenStandardProgram(episode.LocalPath, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnPlayEpisode Error!");
            }
        }

        private void ScanFolder()
        {
            try
            {
                Thread.Sleep(10);
                List<TvShow> tvShows = _fileHandler.ScanLocalTvShows();
                if (tvShows != null)
                    _dbHandler.SyncLocalTvShows(tvShows);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnDeleteEpisode Error!");
            }
        }

        private void OnDeleteEpisode(object sender, object e)
        {
            try
            {
                _fileHandler.DeleteEpisode((Episode)e);
                _dbHandler.DeleteEpisode((Episode)e);
                _controller.RefreshData();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnDeleteEpisode Error!");
            }
        }

        private void OnDeleteTvShow(object sender, object e)
        {
            try
            {
                _fileHandler.DeleteTvShow((TvShow)e);
                _dbHandler.DeleteTvShow((TvShow)e);
                _controller.RefreshData();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnDeleteTvShow Error!");
            }
        }

        private void OnToggleAutoDownload(object sender, object e)
        {
            try
            {
                TvShow tvShow = e as TvShow;
                if (tvShow != null && tvShow.TvShowId.Equals(0))
                    _dbHandler.AddTvShow(tvShow, true);
                else
                    _dbHandler.UpdateTvShow(null, e);
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnToggleAutoDownload Error!");
            }
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
            catch (Exception ex)
            {
                Log.Error(ex,"LoadOrCreateConfig Error!");
            }
        }

        private void RefreshLocalConfig()
        {
            try
            {
                _fileHandler.FileEndings = _config.FileEndings?.Split(';').ToList();
                _fileHandler.LocalPath1 = _config.LocalPath1;
                _fileHandler.LocalPath2 = _config.LocalPath2;
                _fileHandler.LocalPath3 = _config.LocalPath3;
                _fileHandler.IsRecursive = _config.IsRecursive;
                _fileHandler.DownloadFolder = _config.DownloadFolder;
                _fileHandler.CopyToPath = _config.CopyToPath;

                _feedHandler.FeedUrl = _config.FeedUrl;
                _controller.RefreshSettingsView();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"RefreshLocalConfig Error!");
            }
        }

        private void OnSaveConfig(object sender, object e)
        {
            try
            {
                string standardXml = XmlHandler.GetSerializedConfigXml(typeof(Config), _config);
                _fileHandler.OverwriteFile(LogicConstants.StandardXmlName, standardXml, LogicConstants.StandardXmlPath);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"RefreshLocalConfig Error!");
            }
        }

        private void OnRestoreLocalPathSettings(object sender, object e)
        {
            try
            {
                var standardConfig = new Config();
                _config.FileEndings = standardConfig.FileEndings;
                _config.DownloadFolder = standardConfig.DownloadFolder;
                _config.CopyToPath = standardConfig.CopyToPath;
                _config.LocalPath1 = standardConfig.LocalPath1;
                _config.LocalPath2 = standardConfig.LocalPath2;
                _config.LocalPath3 = standardConfig.LocalPath3;
                OnSaveConfig(null, null);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnRestoreLocalPathSettings Error!");
            }
        }

        private void OnRestoreFeedLinkSettings(object sender, object e)
        {
            try
            {
                var standardConfig = new Config();
                _config.FeedUrl = standardConfig.FeedUrl;
                _config.DownloadFolder = standardConfig.DownloadFolder;

                _config.NameFrontRegex = standardConfig.NameFrontRegex;
                _config.NameBackRegex = standardConfig.NameBackRegex;
                _config.NumberFrontRegex = standardConfig.NumberFrontRegex;
                _config.NumberBackRegex = standardConfig.NumberBackRegex;

                _config.TorrentNameRegex = standardConfig.TorrentNameRegex;

                OnSaveConfig(null, null);
                RefreshLocalConfig();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnRestoreFeedLinkSettings Error!");
            }
        }

        private void OnLogRefresh(object sender, object e)
        {
            try
            {
                _sqLiteLog = _dbHandler?.GetLogFileData();
                _controller.RefreshSettingsView();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"OnLogRefresh Error!");
            }
        }

        private void OnExceptionEvent(object sender, Exception e)
        {
            Log.Error(e,"OnExceptionEvent Error!");
        }
    }
}
