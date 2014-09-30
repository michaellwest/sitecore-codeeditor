using System;
using System.Text.RegularExpressions;

namespace Sitecore.SharedSource.Data
{
    public class HtmlUtil
    {
        public static string ReplaceLineBreaks(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"(\r\n|\n)", "<br />", RegexOptions.Compiled);
        }

        public static string ReplaceHtmlBreaks(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"(<br>)|(<br */>)|(\[br */\])", "  \n", RegexOptions.Compiled);
        }
    }
}