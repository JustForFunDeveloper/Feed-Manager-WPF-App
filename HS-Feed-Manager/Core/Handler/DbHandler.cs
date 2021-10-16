using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HS_Feed_Manager.Core.Data;
using HS_Feed_Manager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace HS_Feed_Manager.Core.Handler
{
    public class DbHandler
    {
        public static List<TvShow> LocalTvShows => _localTvShows;

        private static List<TvShow> _localTvShows;

        public DbHandler()
        {
            try
            {
                _localTvShows = new List<TvShow>();
                using (var db = new ApplicationDbContext())
                {
                    db.Database.Migrate();
                }
                UpdateLocalTvShows();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DbHandler Error!");
            }
        }

        #region Public Methods

        public void UpdateTvShow(object sender, object value)
        {
            try
            {
                if (value == null)
                    return;
                TvShow tvShow = value as TvShow;
                using (var db = new ApplicationDbContext())
                {
                    if (tvShow != null)
                        db.TvShows.Update(tvShow);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"UpdateTvShow Error!");
            }
        }

        public void UpdateEpisode(object sender, object value)
        {
            try
            {
                if (value == null)
                    return;
                Episode episode = (Episode) value;

                using (var db = new ApplicationDbContext())
                {
                    db.Episodes.Update(episode);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"UpdateEpisode Error!");
            }
        }

        public void SyncLocalTvShows(List<TvShow> tvShows)
        {
            try
            {
                UpdateLocalTvShows();
                // Sync from db an check if files exists locally and set the correct counter
                foreach (var localTvShow in LocalTvShows)
                {
                    int existsCounter = 0;
                    foreach (var episode in localTvShow.Episodes)
                    {
                        if (episode.LocalPath == null)
                            continue;
                        if (File.Exists(episode.LocalPath))
                            existsCounter++;
                        else
                            episode.LocalPath = null;
                    }

                    localTvShow.LocalEpisodesCount = existsCounter;
                    UpdateTvShow(null, localTvShow);
                }

                // Sync from new scan to db and add new shows
                foreach (var tvShow in tvShows)
                {
                    TvShow existingTvShow = LocalTvShows.SingleOrDefault(item => item.Name.Equals(tvShow.Name));
                    if (existingTvShow == null)
                    {
                        tvShow.LocalEpisodesCount = tvShow.Episodes.Count;
                        AddTvShow(tvShow);
                    }
                    else
                    {
                        foreach (var episode in tvShow.Episodes)
                        {
                            if (!existingTvShow.Episodes.Any(item =>
                                item.Name.Equals(episode.Name) && item.EpisodeNumber.Equals(episode.EpisodeNumber)))
                            {
                                existingTvShow.Episodes.Add(episode);
                            }
                            else
                            {
                                Episode localEpisode =
                                    existingTvShow.Episodes.SingleOrDefault(item =>
                                        item.Name.Equals(episode.Name) &&
                                        item.EpisodeNumber.Equals(episode.EpisodeNumber));

                                if (localEpisode == null)
                                    continue;

                                if (localEpisode.LocalPath == null)
                                {
                                    localEpisode.LocalPath = episode.LocalPath;
                                }
                                else if (!localEpisode.LocalPath.Equals(episode.LocalPath))
                                {
                                    localEpisode.LocalPath = episode.LocalPath;
                                }
                            }
                        }

                        UpdateTvShow(null, existingTvShow);
                    }
                }

                UpdateLocalTvShows();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"SyncLocalTvShows Error!");
            }
        }

        public void AddTvShow(TvShow tvShow, bool updateNow = false)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.TvShows.Add(tvShow);
                    db.SaveChanges();
                }
                if(updateNow)
                    UpdateLocalTvShows();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"AddTvShow Error!");
            }
        }

        public void DeleteEpisode(Episode episode)
        {
            try
            {
                TvShow localTvShow = LocalTvShows.SingleOrDefault(item => item.Name.Equals(episode.Name));
                if (localTvShow != null)
                {
                    Episode localEpisode = localTvShow.Episodes.SingleOrDefault(epi => epi.EpisodeNumber.Equals(episode.EpisodeNumber));
                    if (localEpisode != null)
                    {
                        localTvShow.LocalEpisodesCount--;
                        using (var db = new ApplicationDbContext())
                        {
                            db.Episodes.Remove(episode);
                            db.Update(localTvShow);
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DeleteEpisode Error!");
            }
        }

        public void DeleteTvShow(TvShow tvShow)
        {
            try
            {
                TvShow localTvShow = LocalTvShows.SingleOrDefault(item => item.Name.Equals(tvShow.Name));
                if (localTvShow != null)
                {
                    using (var db = new ApplicationDbContext())
                    {
                        foreach (var episode in localTvShow.Episodes)
                        {
                            db.Episodes.Remove(episode);
                        }

                        db.TvShows.Remove(localTvShow);
                        db.SaveChanges();
                    }
                }
                UpdateLocalTvShows();
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DeleteTvShow Error!");
            }
        }

        public string GetLogFileData()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var sqLiteLogs = db.Logs.OrderByDescending(l => l.TimeStamp).Take(100).ToList();
                    StringBuilder builder = new StringBuilder();
                    foreach (var sqLiteLog in sqLiteLogs)
                    {
                        builder
                            .Append(sqLiteLog.TimeStamp).Append("\t")
                            .Append(sqLiteLog.Level).Append("\t")
                            .Append(sqLiteLog.RenderedMessage).Append("\t")
                            .Append(sqLiteLog.Exception).Append("\n");
                    }

                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"DeleteTvShow Error!");
                return "";
            }
        }
        
        #endregion

        #region Private Methods

        private void UpdateLocalTvShows()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    _localTvShows.Clear();
                    _localTvShows.AddRange(db.TvShows
                        .Include(tvShow => tvShow.Episodes)
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex,"UpdateLocalTvShows Error!");
            }
        }

        #endregion
    }
}