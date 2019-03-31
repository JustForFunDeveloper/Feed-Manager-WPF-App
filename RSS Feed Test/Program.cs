using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;

namespace RSS_Feed_Test
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlTextReader reader = new XmlTextReader("http://www.horriblesubs.info/rss.php?res=720");
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            List<SyndicationItem> items = new List<SyndicationItem>(feed.Items);
            foreach (SyndicationItem item in items)
            {
                GetTitles(item);
                Console.WriteLine("Episode: " + item.Title.Text);
                Console.WriteLine("Links: " + item.Links[0].Uri);
                //StartDownload(item.Links[0].Uri.ToString());
                break;
            }
            Console.ReadLine();
        }

        static void GetTitles(SyndicationItem item)
        {
            string name = item.Title.Text;
            name = name.Replace("[HorribleSubs] ", "");
            string regex = @" - [0-9][0-9]* \[720p].mkv";
            name = Regex.Replace(name, regex, "");
            Console.WriteLine(name);
        }

        static void StartDownload(string uri)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(uri);
            Process.Start(sInfo);
        }
    }
}