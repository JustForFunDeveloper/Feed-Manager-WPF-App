using System.Text.RegularExpressions;

namespace HS_Feed_Manager.Core.Handler
{
    // TODO: Better Logging
    public class FileNameParser
    {
        #region Public Properties

        public string NameFrontRegex { get; set; }
        public string NameBackRegex { get; set; }
        public string NumberFrontRegex { get; set; }
        public string NumberBackRegex { get; set; }

        #endregion

        public FileNameParser()
        {
            //NameFrontRegex = @"\[HorribleSubs] ";
            //NameBackRegex = @" - [0-9]*.[0-9] \[720p].mkv";
            //NumberFrontRegex = @"\[HorribleSubs] .* - ";
            //NumberBackRegex = @" \[720p].mkv";
        }

        public string GetNameFromFeedItem(string feedTitle)
        {
            string temp = Regex.Replace(feedTitle, NameFrontRegex, "");
            return Regex.Replace(temp, NameBackRegex, "");
        }

        public double GetEpisodeNumberFromFeedItem(string feedTitle)
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
    }
}
