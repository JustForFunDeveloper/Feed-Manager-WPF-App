using System;
using System.Collections.Generic;

namespace HS_Feed_Manager.DataModels
{
    public static class CurrenData
    {
        public static List<Episode> FeedList { get; set; }
        public static List<LocalSeries> LocalList { get; set; }

        public static void Seed()
        {
            if (FeedList != null)
                return;

            FeedList = new List<Episode>
            {
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 100, Link = "http://www.google.at" },
                new Episode { Name = "Gege no Kitaro", EpisodeNumber = 22, Link = "http://www.google.at" },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 303, Link = "http://www.google.at" },
                new Episode { Name = "Kono Oto Tomare", EpisodeNumber = 100, Link = "http://www.google.at" },
                new Episode { Name = "Bokutachi wa Benkyou ga Dekinai", EpisodeNumber = 1, Link = "http://www.google.at" },
                new Episode { Name = "Nobunaga-sensei no Osanazuma", EpisodeNumber = 1, Link = "http://www.google.at" },
                new Episode { Name = "Kimetsu no Yaiba", EpisodeNumber = 1, Link = "http://www.google.at" },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 1, Link = "http://www.google.at" }
            };

            List<Episode> localEpisodes1 = new List<Episode>
            {
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 10, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddSeconds(1), Rating = 4 },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 11, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(2), Rating = 4  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 12, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddDays(3), Rating = 1  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 103, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddHours(4), Rating = 1  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 104, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(5), Rating = 2  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 105, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(6), Rating = 2  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 106, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(7), Rating = 3  },
                new Episode { Name = "Boruto - Next Generations", EpisodeNumber = 107, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(8), Rating = 3  }
            };

            List<Episode> localEpisodes2 = new List<Episode>
            {
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 10, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddDays(1), Rating = 1 },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 11, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddHours(2), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 12, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddSeconds(3), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 13, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 14, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 15, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 16, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 17, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 18, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 19, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 21, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 20, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  },
                new Episode { Name = "Fairy Tail Final Season", EpisodeNumber = 101, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(4), Rating = 1  }
            };

            List<Episode> localEpisodes3 = new List<Episode>
            {
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 10, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddSeconds(2), Rating = 0  },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 11, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(3), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 12, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddDays(4), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 103, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddHours(5), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 104, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(6), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 105, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(7), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 106, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(8), Rating = 0 },
                new Episode { Name = "Ace of Diamond Act II", EpisodeNumber = 107, Link = "http://www.google.at", DownloadDate = DateTime.Now.AddMinutes(9), Rating = 0 }
            };

            LocalList = new List<LocalSeries>
            {
                new LocalSeries
                {
                    Name = "Boruto - Next Generations", EpisodeList = localEpisodes1, Status = Status.Ongoing, Episodes = 200,
                    AutoDownloadStatus = AutoDownload.On, LocalEpisodesCount = localEpisodes1.Count, Rating = 4, LatestDownload = localEpisodes1[0].DownloadDate
                },
                new LocalSeries
                {
                    Name = "Fairy Tail Final Season", EpisodeList = localEpisodes2, Status = Status.Finished, Episodes = 13,
                    AutoDownloadStatus = AutoDownload.Off, LocalEpisodesCount = localEpisodes2.Count, Rating = 5, LatestDownload = localEpisodes2[2].DownloadDate
                },
                new LocalSeries
                {
                    Name = "Ace of Diamond Act II", EpisodeList = localEpisodes3, Status = Status.Ongoing, Episodes = 24,
                    AutoDownloadStatus = AutoDownload.On, LocalEpisodesCount = localEpisodes3.Count, Rating = 0, LatestDownload = localEpisodes3[0].DownloadDate
                }
            };
        }
    }
}
