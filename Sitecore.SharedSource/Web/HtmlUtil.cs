using System.Text.RegularExpressions;

namespace Sitecore.SharedSource.Web
{
    public class HtmlUtil
    {
        public static string ReplaceNewLines(string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"(\r\n|\n)", "<br />", RegexOptions.Compiled);
        }

        public static string ReplaceLineBreaks(string input)
        {
            return string.IsNullOrEmpty(input) ? input : Regex.Replace(input, @"(<br>)|(<br */>)|(\[br */\])", "  \n", RegexOptions.Compiled);
        }
    }
}