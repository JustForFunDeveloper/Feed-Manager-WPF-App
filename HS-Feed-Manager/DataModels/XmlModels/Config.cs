using System.Xml.Serialization;
using HS_Feed_Manager.Core.Handler;

namespace HS_Feed_Manager.DataModels.XmlModels
{
    [XmlRoot("Data_Handler_Tool_Config")]
    public class Config
    {
        public string FileEndings = "*.mkv;";
        public string DownloadFolder = "";
        public string LocalPath1 = "Z:\\Anime";
        public string LocalPath2 = "";
        public string LocalPath3 = "";

        public string FeedUrl = "https://rss.erai-ddl2.info/rss-720/";
        public string NameFrontRegex = @"(\[720p\]|\[1080p\]) ";
        public string NameBackRegex = @" – [0-9]*.[0-9].*";
        public string NumberFrontRegex = @".* – ";
        public string NumberBackRegex = @" END*| \(GP Version\)*| \(V0\)*| \(V1\)*| \(V2\)*| \(Multi\)*| \(Special Preview\)*";

        public string FileNameFrontRegex = @"\[Erai-raws\] |\[HorribleSubs\] ";
        public string FileNameBackRegex = @" - [0-9]*.[0-9].*";
        public string FileNumberFrontRegex = @".* - ";
        public string FileNumberBackRegex = @"([END]*[\(Special Preview\)]* [\[(v0|v1|v2)\]]*(\[480p\]|\[720p\]|\[1080p\]))(\[Multiple Subtitle\]*|).mkv";

        public string TorrentNameRegex = @"[!a-z0-9A-Z-\[\] ]*\.mkv\.torrent";

        public LogLevel Level = LogLevel.Debug;
    }
}
