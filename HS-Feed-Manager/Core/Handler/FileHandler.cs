using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using HS_Feed_Manager.DataModels.DbModels;
using HS_Feed_Manager.ViewModels.Handler;
using Serilog;
using Path = System.IO.Path;

namespace HS_Feed_Manager.Core.Handler
{
    public class FileHandler
    {
        #region Public Properties

        public string LocalPath1 { get; set; }
        public string LocalPath2 { get; set; }
        public string LocalPath3 { get; set; }
        public bool IsRecursive { get; set; }
        public string DownloadFolder { get; set; }
        public string CopyToPath { get; set; }
        public List<string> FileEndings { get; set; }
        /// <summary>
        /// An event which is invoked as soon a exception occured.<para/>
        /// Used to log Exceptions in this Handler.
        /// </summary>
        public event EventHandler<Exception> ExceptionEvent;

        #endregion

        #region Public Methods

        public List<TvShow> ScanLocalTvShows()
        {
            try
            {
                List<TvShow> tvShows = null;
                List<FileInfo> fileInfos = new List<FileInfo>();

                if (LocalPath1.Length > 0)
                    fileInfos.AddRange(SearchDirectory(LocalPath1));

                if (LocalPath2.Length > 0)
                    fileInfos.AddRange(SearchDirectory(LocalPath2));

                if (LocalPath3.Length > 0)
                    fileInfos.AddRange(SearchDirectory(LocalPath3));

                if (fileInfos.Count > 0)
                {
                    tvShows = GetEpisodesFromFileList(fileInfos);
                }

                return tvShows;
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return null;
            }
        }

        public void OpenStandardProgram(string path, bool islocalPath)
        {
            try
            {
                if (islocalPath)
                {
                    if (File.Exists(path))
                    {
                        ProcessStartInfo sInfo = new ProcessStartInfo(path);
                        Process.Start(sInfo);
                    }
                }
                else
                {
                    ProcessStartInfo sInfo = new ProcessStartInfo(path);
                    Process.Start(sInfo);
                }
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
        }

        /// <summary>
        /// This methods creates a file and adds a basic header if withHeader is true.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <param name="withHeader">Adds a header if true.</param>
        /// <returns>Returns -1 if something went wrong and returns 1 if file did exist.</returns>
        public short CreateFileIfNotExist(string fileName, string path = "", bool withHeader = true)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (fileInfo.Exists)
                    return 1;

                DateTime dt = DateTime.Now;
                string[] lines = { fileName, "Created at: " + dt.ToLocalTime().ToString() };

                if (withHeader)
                    File.WriteAllLines(path + fileName, lines);
                else
                    File.WriteAllText(path + fileName, "");
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This methods creates a folder if it doesn't exist.
        /// </summary>
        /// <param name="path">The file name.</param>
        /// <returns>Returns -1 if something went wrong and returns 1 if file did exist.</returns>
        public short CreateFolderNotExist(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                    return 1;
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method appends a given line to the given file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="text">The given text to append.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <param name="withNewline"></param>
        /// <returns>Returns -1 if something went wrong.</returns>
        public short AppendText(string fileName, string text, string path = "", bool withNewline = true)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                using (StreamWriter file = new StreamWriter(path + fileName, true))
                {
                    if (withNewline)
                        file.WriteLine(text);
                    else
                        file.Write(text);
                }
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// Reads all lines of the text and returns it
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns>Returns null if something went wrong.</returns>
        public string ReadAllText(string fileName, string path = "")
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    return null;

                return File.ReadAllText(path + fileName);
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return null;
            }
        }

        /// <summary>
        /// This method overwrites a given text to the given file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <param name="text">The given text to overwrite.</param>
        /// <param name="path">The given path. Should end with '\'.</param>
        /// <returns>Returns -1 if something went wrong.</returns>
        public short OverwriteFile(string fileName, string text, string path = "")
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path + fileName);
                if (!fileInfo.Exists)
                    throw new Exception("File doesn't exist!");

                File.WriteAllText(path + fileName, text);
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// This method gets all shows from the download folder.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns>A list of <see cref="TvShow"/></returns>
        public List<TvShow> GetAndCopyDownloadedShows(CancellationToken ct)
        {
            List<TvShow> tvShows = null;
            List<FileInfo> fileInfos = new List<FileInfo>();

            if (DownloadFolder.Length > 0)
                fileInfos.AddRange(SearchDirectory(DownloadFolder));
            else
            {
                return null;
            }

            if (fileInfos.Count > 0)
            {
                tvShows = GetEpisodesFromFileList(fileInfos);
            }

            if (tvShows == null || tvShows.Count <= 0)
            {
                return null;
            }

            Log.Information($"Started to Copy {fileInfos.Count} Episodes");
            
            int counter = 0;
            foreach (var fileInfo in fileInfos)
            {
                if (ct.IsCancellationRequested)
                {
                    Log.Information("Canceled Copy Downloaded Shows");
                    return tvShows;
                }
                
                var tvShow = tvShows.Single(tv => tv.Episodes.Any(e => e.LocalPath == fileInfo.FullName));
                var episode = tvShow.Episodes.Single(ep => ep.LocalPath == fileInfo.FullName);
                var directoryPath = Path.Combine(CopyToPath, tvShow.Name);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var destPath = Path.Combine(directoryPath, fileInfo.Name);
                fileInfo.MoveTo(destPath);
                episode.LocalPath = destPath;
                counter++;
                var progressMessage = $"Finished {counter} of {fileInfos.Count}";
                Mediator.NotifyColleagues(MediatorGlobal.ProgressMessage, progressMessage);
            }

            return tvShows;
        }

        #endregion

        #region Private Methods

        private List<TvShow> GetEpisodesFromFileList(List<FileInfo> fileInfos)
        {
            try
            {
                List<TvShow> tvShows = new List<TvShow>();

                foreach (var fileInfo in fileInfos)
                {
                    Episode episode = new Episode()
                    {
                        Name = FileNameParser.GetNameFromItem(fileInfo.Name),
                        LocalPath = fileInfo.FullName
                    };

                    double episodeNumber = FileNameParser.GetEpisodeNumberFromItem(fileInfo.Name);
                    if (episodeNumber.Equals(-1))
                    {
                        OnExceptionEvent(new FileFormatException("Can't parse Local Episode-number from: " + fileInfo.Name));
                        continue;
                    }

                    episode.EpisodeNumber = episodeNumber;

                    TvShow tvShow = tvShows.SingleOrDefault(item => item.Name.Equals(episode.Name));
                    if (tvShow == null)
                    {
                        tvShows.Add(new TvShow()
                        {
                            Name = episode.Name,
                            Episodes = new List<Episode>() { episode }
                        });
                    }
                    else
                    {
                        tvShow.Episodes.Add(episode);
                    }
                }

                return tvShows;
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return null;
            }
        }

        private IEnumerable<FileInfo> SearchDirectory(string path)
        {
            try
            {
                var searchOption = SearchOption.TopDirectoryOnly;
                if (IsRecursive)
                {
                    searchOption = SearchOption.AllDirectories;
                }
                
                var strings = Directory.GetFiles(path, "*", searchOption);
                return strings.Select(s => new FileInfo(s)).ToList();
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
                return null;
            }
        }

        public void DeleteEpisode(Episode episode)
        {
            try
            {
                if (File.Exists(episode.LocalPath))
                {
                    File.Delete(episode.LocalPath);
                }
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
        }

        public void DeleteTvShow(TvShow tvShow)
        {
            try
            {
                if (tvShow != null)
                {
                    foreach (var tvShowEpisode in tvShow.Episodes)
                    {
                        if (File.Exists(tvShowEpisode.LocalPath))
                        {
                            File.Delete(tvShowEpisode.LocalPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
        }

        public void OpenFolder(Episode episode)
        {
            try
            {
                if (File.Exists(episode.LocalPath))
                {
                    Process.Start("explorer.exe", "/select, " + episode.LocalPath);
                }
            }
            catch (Exception e)
            {
                OnExceptionEvent(e);
            }
        }

        #endregion

        #region Invoke Methods

        /// <summary>
        /// <see cref="ExceptionEvent"/>
        /// </summary>
        /// <param name="e"><see cref="ExceptionEvent"/></param>
        protected virtual void OnExceptionEvent(Exception e)
        {
            ExceptionEvent?.Invoke(this, e);
        }

        #endregion
    }
}
