using System;
using System.Collections.Generic;

namespace HS_Feed_Manager.DataModels.DbModels
{
    public class TvShow
    {
        public int TvShowId { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public int EpisodeCount { get; set; }
        public AutoDownload AutoDownloadStatus { get; set; }
        public int LocalEpisodesCount { get; set; }
        public int Rating { get; set; }
        public DateTime LatestDownload { get; set; }

        public ICollection<Episode> Episodes { get; set; }
    }
}
