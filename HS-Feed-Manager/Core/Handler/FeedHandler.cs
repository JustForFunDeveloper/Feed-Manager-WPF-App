using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using HS_Feed_Manager.DataModels.DbModels;

namespace HS_Feed_Manager.Core.Handler
{
    // TODO: Better Logging
    public class FeedHandler
    {
        #region Public Properties

        public string FeedUrl { get; set; }

        #endregion

        #region Private Properties

        private FileNameParser _fileNameParser;

        #endregion

        public FeedHandler(string feedUrl, FileNameParser fileNameParser)
        {
            FeedUrl = feedUrl;
            _fileNameParser = fileNameParser;
        }

        public FeedHandler(FileNameParser fileNameParser)
        {
            _fileNameParser = fileNameParser;
            FeedUrl = "http://www.horriblesubs.info/rss.php?res=720";
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
                        // TODO: Create better Logging
                        Console.WriteLine("Can't parse Feed Episode-number from: " + syndicationItem.Title.Text);
                    }
                    else
                    {
                        episode.EpisodeNumber = episodeNumber;
                    }

                    episodes.Add(episode);
                }

                return episodes;
            }
            catch (Exception e)
            {
                // TODO: Write in error Log!
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
