using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core.Handler
{
    public class FileHandler
    {
        // TODO: Better Logging
        #region Public Methods

        public string LocalPath1 { get; set; }
        public string LocalPath2 { get; set; }
        public string LocalPath3 { get; set; }
        public List<string> FileEndings { get; set; }

        #endregion

        #region Private Methods

        private FileNameParser _fileNameParser;

        #endregion

        public FileHandler(FileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
            LocalPath1 = @"D:\Anime\";
            LocalPath2 = "";
            LocalPath3 = "";
            FileEndings = new List<string> { "*.mkv" };
        }

        #region Public Methods

        public List<TvShow> ScanLocalTvShows()
        {
            try
            {
                List<TvShow> tvShows = null;
                List<FileInfo> fileInfos = new List<FileInfo>();

                if (LocalPath1.Length > 0)
                    fileInfos.AddRange(GetLocalFileNames(LocalPath1));

                if (LocalPath2.Length > 0)
                    fileInfos.AddRange(GetLocalFileNames(LocalPath2));

                if (LocalPath2.Length > 0)
                    fileInfos.AddRange(GetLocalFileNames(LocalPath3));

                if (fileInfos.Count > 0)
                {
                    tvShows = GetEpisodesFromFileList(fileInfos);
                }

                return tvShows;
            }
            catch (Exception e)
            {
                // TODO: Write in error Log!
                Console.WriteLine(e);
                return null;
            }
        }

        public void OpenStandardProgram(string path)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(path);
            Process.Start(sInfo);
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
                        Name = _fileNameParser.GetNameFromFeedItem(fileInfo.Name),
                        LocalPath = fileInfo.FullName
                    };

                    double episodeNumber = _fileNameParser.GetEpisodeNumberFromFeedItem(fileInfo.Name);
                    if (episodeNumber.Equals(-1))
                    {
                        // TODO: Better Logging
                        Console.WriteLine("Can't parse Local Episode-number from: " + fileInfo.Name);
                        continue;
                    }
                    else
                    {
                        episode.EpisodeNumber = episodeNumber;
                    }

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
                // TODO: Write in error Log!
                Console.WriteLine(e);
                return null;
            }
        }

        private IEnumerable<FileInfo> GetLocalFileNames(string localPath1)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            foreach (var fileEnding in FileEndings)
            {
                DirectoryInfo d = new DirectoryInfo(localPath1);
                FileInfo[] files = d.GetFiles(fileEnding);
                foreach (var fileInfo in files)
                {
                    fileInfos.Add(fileInfo);
                }
            }

            return fileInfos;
        }

        public void DeleteEpisode(Episode episode)
        {
            if (File.Exists(episode.LocalPath))
            {
                File.Delete(episode.LocalPath);
            }
        }

        public void DeleteTvShow(TvShow tvShow)
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

        public void OpenFolder(Episode episode)
        {
            if (File.Exists(episode.LocalPath))
            {
                Process.Start("explorer.exe", "/select, " + episode.LocalPath);
            }
        }

        #endregion
    }
}
