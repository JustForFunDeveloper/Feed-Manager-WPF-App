using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using CloudflareSolverRe;
using HS_Feed_Manager.DataModels.DbModels;
using Serilog;

namespace HS_Feed_Manager.Core.Handler
{
    public class FeedHandler
    {
        #region Public Properties

        public string FeedUrl { get; set; }

        #endregion

        public async Task<List<Episode>> DownloadFeedList()
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
                client.Timeout = TimeSpan.FromSeconds(5);

                var content = await client.GetStringAsync(FeedUrl);
                
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
                        Log.Warning("Can't parse Feed Episode-number from: " + syndicationItem.Links[0].Uri);
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
                Log.Error(ex,"DownloadFeedList Error!");
                return null;
            }
        }
    }
}
