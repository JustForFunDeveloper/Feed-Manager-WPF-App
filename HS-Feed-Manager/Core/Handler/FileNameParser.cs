using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HS_Feed_Manager.Core.Handler
{
    public static class FileNameParser
    {
        public static string GetNameFromFeedItem(string feedTitle)
        {
            try
            {
                string temp = Regex.Replace(feedTitle, Logic.LocalConfig.NameFrontRegex, "");
                return Regex.Replace(temp, Logic.LocalConfig.NameBackRegex, "");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetNameFromFeedItem: " + ex, LogLevel.Error);
                return null;
            }
        }

        public static double GetEpisodeNumberFromFeedItem(string feedTitle)
        {
            try
            {
                string temp = Regex.Replace(feedTitle, Logic.LocalConfig.NumberFrontRegex, "");
                string value = Regex.Replace(temp, Logic.LocalConfig.NumberBackRegex, "");

                if (value.Contains("v"))
                {
                    value = value.Replace("v", ".");
                }
                if (value.Contains("."))
                {
                    value = value.Replace(".", ",");
                }
                if (double.TryParse(value, out double returnValue))
                    return returnValue;
                else
                    return -1;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetEpisodeNumberFromFeedItem: " + ex, LogLevel.Error);
                return -1;
            }
        }

        public static string GetNameFromFileItem(string fileTitle)
        {
            try
            {
                string temp = Regex.Replace(fileTitle, Logic.LocalConfig.FileNameFrontRegex, "");
                return Regex.Replace(temp, Logic.LocalConfig.FileNameBackRegex, "");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetNameFromFileItem: " + ex, LogLevel.Error);
                return null;
            }
        }

        public static double GetEpisodeNumberFromFileItem(string fileTitle)
        {
            try
            {
                string temp = Regex.Replace(fileTitle, Logic.LocalConfig.FileNumberFrontRegex, "");
                string value = Regex.Replace(temp, Logic.LocalConfig.FileNumberBackRegex, "");

                if (value.Contains("v"))
                {
                    value = value.Replace("v", ".");
                }
                if (value.Contains("."))
                {
                    value = value.Replace(".", ",");
                }
                if (double.TryParse(value, out double returnValue))
                    return returnValue;
                else
                    return -1;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetEpisodeNumberFromFileItem: " + ex, LogLevel.Error);
                return -1;
            }
        }

        public static string TorrentNameParser(string fileTitle)
        {
            try
            {
                string name = Regex.Match(fileTitle, Logic.LocalConfig.TorrentNameRegex).Value;

                if (name.Length <= 0)
                {
                    new InvalidDataException("Empty torrent string.");
                }

                Regex rgx = new Regex(@"[/]");
                if (rgx.IsMatch(name))
                {
                    new InvalidDataException($"Invalid torrent name {name}");
                }

                return name;
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("TorrentNameRegex: " + ex, LogLevel.Error);
                return null;
            }
        }
    }
}
