using System;
using System.Text.RegularExpressions;

namespace HS_Feed_Manager.Core.Handler
{
    public class FileNameParser
    {
        #region Public Properties

        public string NameFrontRegex { get; set; }
        public string NameBackRegex { get; set; }
        public string NumberFrontRegex { get; set; }
        public string NumberBackRegex { get; set; }

        #endregion

        public string GetNameFromFeedItem(string feedTitle)
        {
            try
            {
                string temp = Regex.Replace(feedTitle, NameFrontRegex, "");
                return Regex.Replace(temp, NameBackRegex, "");
            }
            catch (Exception ex)
            {
                LogHandler.WriteSystemLog("GetNameFromFeedItem: " + ex.ToString(), LogLevel.Error);
                return null;
            }
        }

        public double GetEpisodeNumberFromFeedItem(string feedTitle)
        {
            try
            {
                string temp = Regex.Replace(feedTitle, NumberFrontRegex, "");
                string value = Regex.Replace(temp, NumberBackRegex, "");

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
                LogHandler.WriteSystemLog("GetEpisodeNumberFromFeedItem: " + ex.ToString(), LogLevel.Error);
                return -1;
            }
        }
    }
}
