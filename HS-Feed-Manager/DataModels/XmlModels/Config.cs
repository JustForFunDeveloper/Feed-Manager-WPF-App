using System.Xml.Serialization;
using HS_Feed_Manager.Core.Handler;

namespace HS_Feed_Manager.DataModels.XmlModels
{
    [XmlRoot("Data_Handler_Tool_Config")]
    public class Config
    {
        [XmlAttribute]
        public string FileEndings = "*.mkv;";
        [XmlAttribute]
        public string LocalPath1 = "";
        [XmlAttribute]
        public string LocalPath2 = "";
        [XmlAttribute]
        public string LocalPath3 = "";

        [XmlAttribute]
        public string FeedUrl = "http://www.horriblesubs.info/rss.php?res=720";
        [XmlAttribute]
        public string NameFrontRegex = @"\[HorribleSubs] ";
        [XmlAttribute]
        public string NameBackRegex = @" - [0-9]*.[0-9] \[720p].mkv";
        [XmlAttribute]
        public string NumberFrontRegex = @"\[HorribleSubs] .* - ";
        [XmlAttribute]
        public string NumberBackRegex = @" \[720p].mkv";

        public DebugLevel DebugLevel = new DebugLevel();
    }

    public class DebugLevel
    {
        [XmlAttribute]
        public LogLevel Level = LogLevel.Debug;
    }
}
