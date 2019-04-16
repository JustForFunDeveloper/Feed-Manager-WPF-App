using System;

namespace HS_Feed_Manager.DataModels.DbModels
{
    public class Episode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double EpisodeNumber { get; set; }
        public string Link { get; set; }
        public DateTime DownloadDate { get; set; }
        public int Rating { get; set; }
        public string LocalPath { get; set; }

        public int TvShowId { get; set; }
        public TvShow TvShow { get; set; }
    }
}
