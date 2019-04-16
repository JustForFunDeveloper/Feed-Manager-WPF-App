using System;
using System.Collections.Generic;
using System.Linq;
using HS_Feed_Manager.Core.Data;
using HS_Feed_Manager.DataModels;
using HS_Feed_Manager.DataModels.DbModels;
using Microsoft.EntityFrameworkCore;

namespace HS_Feed_Manager.Core.Handler
{
    public class DbHandler
    {
        public static IEnumerable<TvShow> LocalTvShows
        {
            get
            {
                using (var db = new ApplicationDbContext())
                {
                    return db.TvShows
                        .Include(tvShow => tvShow.Episodes)
                        .ToList();
                }
            }
        }

        #region Test Data   

        private static readonly List<Episode> LocalEpisodes1 = new List<Episode>
        {
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 10, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddSeconds(1), Rating = 4
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 11, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(2), Rating = 4
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 12, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddDays(3), Rating = 1
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 103, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddHours(4), Rating = 1
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 104, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(5), Rating = 2
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 105, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(6), Rating = 2
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 106, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(7), Rating = 3
            },
            new Episode
            {
                Name = "Boruto - Next Generations", EpisodeNumber = 107, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(8), Rating = 3
            }
        };

        private static readonly List<Episode> LocalEpisodes2 = new List<Episode>
        {
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 10, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddDays(1), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 11, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddHours(2), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 12, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddSeconds(3), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 13, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 14, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 15, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 16, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 17, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 18, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 19, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 21, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 20, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            },
            new Episode
            {
                Name = "Fairy Tail Final Season", EpisodeNumber = 101, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1
            }
        };

        private static readonly List<Episode> LocalEpisodes3 = new List<Episode>
        {
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 10, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddSeconds(2), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 11, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(3), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 12, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddDays(4), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 103, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddHours(5), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 104, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(6), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 105, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(7), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 106, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(8), Rating = 0
            },
            new Episode
            {
                Name = "Ace of Diamond Act II", EpisodeNumber = 107, Link = "http://www.google.at",
                DownloadDate = DateTime.Now.AddMinutes(9), Rating = 0
            }
        };

        private List<TvShow> _tvShows = new List<TvShow>
        {
            new TvShow
            {
                Name = "Boruto - Next Generations", Episodes = LocalEpisodes1, Status = Status.Ongoing,
                EpisodeCount = 200,
                AutoDownloadStatus = AutoDownload.On, LocalEpisodesCount = LocalEpisodes1.Count, Rating = 4,
                LatestDownload = LocalEpisodes1[0].DownloadDate
            },
            new TvShow
            {
                Name = "Fairy Tail Final Season", Episodes = LocalEpisodes2, Status = Status.Finished,
                EpisodeCount = 13,
                AutoDownloadStatus = AutoDownload.Off, LocalEpisodesCount = LocalEpisodes2.Count, Rating = 5,
                LatestDownload = LocalEpisodes2[2].DownloadDate
            },
            new TvShow
            {
                Name = "Ace of Diamond Act II", Episodes = LocalEpisodes3, Status = Status.Ongoing, EpisodeCount = 24,
                AutoDownloadStatus = AutoDownload.On, LocalEpisodesCount = LocalEpisodes3.Count, Rating = 0,
                LatestDownload = LocalEpisodes3[0].DownloadDate
            }
        };

        #endregion

        public DbHandler()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    db.Database.Migrate();
                    // Create TestData Once
                    if (db.TvShows.ToList().Count == 0)
                    {
                        db.TvShows.AddRange(_tvShows);
                        db.SaveChanges();
                    }

                    #region Tests

                    //var query =
                    //    from ep in db.Episodes
                    //    from tv in db.TvShows
                    //    where ep.Id == tv.TvShowId && (tv.Name.Equals("Boruto - Next Generations")) &&
                    //          (ep.EpisodeNumber.Equals(10))
                    //    select new
                    //    {
                    //        EpisodeId = ep.Id,
                    //        TvId = tv.TvShowId,
                    //        EpName = ep.Name
                    //    };

                    //foreach (var epi in query)
                    //{
                    //    Console.WriteLine($"{epi.EpisodeId} | " + 
                    //                      $"{epi.TvId} | " + 
                    //                      $"{epi.EpName}");
                    //}

                    //      db.Episodes.Update(episode);
                    //      db.SaveChanges();

                    //try
                    //{
                    //    var episode = db.Episodes.Single(x => x.TvShow.Name.Equals("Boruto - Next Generations") && x.EpisodeNumber.Equals(10));
                    //    episode.EpisodeNumber = 66;

                    //    db.Episodes.Update(episode);
                    //    db.SaveChanges();
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine(e);
                    //}

                    #endregion
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateEpisode(object sender, object value)
        {
            try
            {
                if (value == null)
                    return;
                Episode episode = (Episode)value;

                using (var db = new ApplicationDbContext())
                {
                    db.Episodes.Update(episode);
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}