using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CloudflareSolverRe;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core.Handler
{
    public class FeedHandler
    {
        #region Public Properties

        public string FeedUrl { get; set; }

        #endregion

        public List<Episode> DownloadFeedList()
        {
            try
            {
                List<Episode> episodes = new List<Episode>();

                // Cloudflare solver
                var handler = new ClearanceHandler
                {
                    MaxTries = 3,
                    ClearanceDelay = 3000
                };

                var client = new HttpClient(handler);

                var content = client.GetStringAsync(FeedUrl).Result;
                
                // Create reader and get items
                XmlTextReader reader = new XmlTextReader(new StringReader(content));
                SyndicationFeed syndicationFeed = SyndicationFeed.Load(reader);
                List<SyndicationItem> feedList = new List<SyndicationItem>(syndicationFeed.Items);

                foreach (SyndicationItem syndicationItem in feedList)
                {
                    Episode episode = new Episode()
                    {
                        // Name = FileNameParser.GetNameFromItem(FileNameParser.TorrentNameParser(syndicationItem.Links[0].Uri.ToString())),
                        Name = FileNameParser.GetNameFromItem(syndicationItem.Title.Text),
                        Link = syndicationItem.Links[0].Uri.ToString()
                    };

                    // double episodeNumber = FileNameParser.GetEpisodeNumberFromItem(FileNameParser.TorrentNameParser(syndicationItem.Links[0].Uri.ToString()));
                    double episodeNumber = FileNameParser.GetEpisodeNumberFromItem(syndicationItem.Title.Text);
                    if (episodeNumber.Equals(-1))
                    {
                        LogHandler.WriteSystemLog("Can't parse Feed Episode-number from: " + syndicationItem.Links[0].Uri, LogLevel.Error);
                    }
                    else
                    {
                        episode.EpisodeNumber = episodeNumber;
                    }

                    episodes.Add(episode);
                }

                return episodes;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("DownloadFeedList: " + ex, LogLevel.Error);
                return null;
            }
        }
    }
}
