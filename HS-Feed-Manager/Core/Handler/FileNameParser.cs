using System;
using System.IO;
using System.Text.RegularExpressions;
using Serilog;

namespace HS_Feed_Manager.Core.Handler
{
    public static class FileNameParser
    {
        public static string GetNameFromItem(string fileTitle)
        {
            try
            {
                string temp = Regex.Replace(fileTitle, Logic.LocalConfig.NameFrontRegex, "");
                string value = Regex.Replace(temp, Logic.LocalConfig.NameBackRegex, "");

                if (value.Length <= 0)
                {
                    new InvalidDataException("Empty File string.");
                }

                return value;
            }
            catch (Exception ex)
            {
                Log.Error(ex,"GetNameFromItem Error!");
                return "";
            }
        }

        public static double GetEpisodeNumberFromItem(string fileTitle)
        {
            try
            {
                string temp = Regex.Replace(fileTitle, Logic.LocalConfig.NumberFrontRegex, "");
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
                Log.Error(ex,"GetEpisodeNumberFromItem Error!");
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
                Log.Error(ex,"TorrentNameRegex Error!");
                return null;
            }
        }
    }
}
