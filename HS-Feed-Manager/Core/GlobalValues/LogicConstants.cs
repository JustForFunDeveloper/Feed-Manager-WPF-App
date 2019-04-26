using System.IO;

namespace HS_Feed_Manager.Core.GlobalValues
{
    public static class LogicConstants
    {
        public static readonly string LogFilePath = Directory.GetCurrentDirectory() + "\\";
        public static readonly string LogFileName = "log.txt";

        public static readonly string StandardXmlPath = Directory.GetCurrentDirectory() + "\\";
        public static readonly string StandardXmlName = "config.xml";

        public static readonly string FileEndings = "*.mkv;";
        public static readonly string LocalPath1 = "";
        public static readonly string LocalPath2 = "";
        public static readonly string LocalPath3 = "";

        public static readonly string FeedUrl = "http://www.horriblesubs.info/rss.php?res=720";
        public static readonly string NameFrontRegex = @"\[HorribleSubs] ";
        public static readonly string NameBackRegex = @" - [0-9]*.[0-9] \[720p].mkv";
        public static readonly string NumberFrontRegex = @"\[HorribleSubs] .* - ";
        public static readonly string NumberBackRegex = @" \[720p].mkv";

        public static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
    }
}
