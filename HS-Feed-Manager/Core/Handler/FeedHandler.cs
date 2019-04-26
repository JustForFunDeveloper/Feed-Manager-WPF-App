using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core.Handler
{
    public class FeedHandler
    {
        #region Public Properties

        public string FeedUrl { get; set; }

        #endregion

        #region Private Properties

        private FileNameParser _fileNameParser;

        #endregion

        public FeedHandler(FileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
        }

        public List<Episode> DownloadFeedList()
        {
            try
            {
                List<Episode> episodes = new List<Episode>();

                // Create reader and get items
                XmlTextReader reader = new XmlTextReader(FeedUrl);
                SyndicationFeed syndicationFeed = SyndicationFeed.Load(reader);
                List<SyndicationItem> feedList = new List<SyndicationItem>(syndicationFeed.Items);

                foreach (SyndicationItem syndicationItem in feedList)
                {
                    Episode episode = new Episode()
                    {
                        Name = _fileNameParser.GetNameFromFeedItem(syndicationItem.Title.Text),
                        Link = syndicationItem.Links[0].Uri.ToString()
                    };

                    double episodeNumber = _fileNameParser.GetEpisodeNumberFromFeedItem(syndicationItem.Title.Text);
                    if (episodeNumber.Equals(-1))
                    {
                        throw new FormatException("Can't parse Feed Episode-number from: " + syndicationItem.Title.Text);
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
                LogHandler.WriteSystemLog("DownloadFeedList: " + ex.ToString(), LogLevel.Error);
                return null;
            }
        }
    }
}
