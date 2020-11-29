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

        public string FeedUrl = "https://subsplease.org/rss/?r=720";

        public string NameFrontRegex = @"\[Erai-raws\] |\[HorribleSubs\] |\[SubsPlease\] ";
        public string NameBackRegex = @" - [0-9]*.[0-9].*";
        public string NumberFrontRegex = @".* (- |\(01-)";
        public string NumberBackRegex = @"([END]*[\(Special Preview\)]* [\[(VRV)\]]*[\[(v0|v1|v2)\]]*(\[480p\]|\[720p\]|\[1080p\]|\(480p\)|\(720p\)|\(1080p\)))(\[Multiple Subtitle\]*|( \[.*\])|)(\.mkv)*(\.torrent)*";

        public string TorrentNameRegex = @"[!\(\)a-z0-9A-Z-\[\] ]*\.mkv\.torrent";

        public LogLevel Level = LogLevel.Debug;
    }
}
