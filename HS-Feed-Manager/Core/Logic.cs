using System.Collections.Generic;
using HS_Feed_Manager.Control;
using HS_Feed_Manager.Core.Handler;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core
{
    public class Logic
    {
        #region Public Properties

        public static readonly List<Episode> FeedEpisodes = new List<Episode>
        {
            new Episode {Name = "Boruto - Next Generations", EpisodeNumber = 100, Link = "http://www.google.at"},
            new Episode {Name = "Gege no Kitaro", EpisodeNumber = 22, Link = "http://www.google.at"},
            new Episode {Name = "Fairy Tail Final Season", EpisodeNumber = 303, Link = "http://www.google.at"},
            new Episode {Name = "Kono Oto Tomare", EpisodeNumber = 100, Link = "http://www.google.at"},
            new Episode {Name = "Bokutachi wa Benkyou ga Dekinai", EpisodeNumber = 1, Link = "http://www.google.at"},
            new Episode {Name = "Nobunaga-sensei no Osanazuma", EpisodeNumber = 1, Link = "http://www.google.at"},
            new Episode {Name = "Kimetsu no Yaiba", EpisodeNumber = 1, Link = "http://www.google.at"},
            new Episode {Name = "Ace of Diamond Act II", EpisodeNumber = 1, Link = "http://www.google.at"}
        };
        public static IEnumerable<TvShow> LocalTvShows => DbHandler.LocalTvShows;

        #endregion

        #region Private Properties

        private DbHandler _dbHandler;
        private Controller _controller;

        #endregion

        public Logic()
        {
            _dbHandler = new DbHandler();
            _controller = new Controller();

            _controller.SaveTvShowData += _dbHandler.UpdateTvShow;
            _controller.SaveEpisodeData += _dbHandler.UpdateEpisode;
        }
    }
}
