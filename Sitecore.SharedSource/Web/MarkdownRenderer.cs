using System;
using Sitecore.Collections;

namespace Sitecore.SharedSource.Web
{
    public class MarkdownRenderer
    {

        public class Options
        {
            public static readonly string SafeMode = "safemode";
            public static readonly string ExtraMode = "extramode";
            public static readonly string MarkdownInHtml = "markdowninhtml";
            public static readonly string AutoHeadingIDs = "autoheadingids";
            public static readonly string NewWindowForExternalLinks = "newwindowforexternallinks";
            public static readonly string NewWindowForLocalLinks = "newwindowforlocallinks";
            public static readonly string NoFollowLinks = "nofollowlinks";
            public static readonly string HtmlClassFootnotes = "htmlclassfootnotes";
            public static readonly string HtmlClassTitledImages = "htmlclasstitledimages";
        }

        public static string Render(string markdown, SafeDictionary<string> parameters)
        {
            var parser = new MarkdownDeep.Markdown();

            // Default values

            // parses all html
            // parser.ExtraMode = true;
            // parser.SafeMode = true;
            // parser.MarkdownInHtml = true;
            parser.NewWindowForExternalLinks = true;
            // parser.NewWindowForLocalLinks = false;
            // parser.NoFollowLinks = false;
            // parser.AutoHeadingIDs = true;

            parser.HtmlClassFootnotes = "";
            parser.HtmlClassTitledImages = "";


            if (HasValue(parameters, Options.ExtraMode))
                parser.ExtraMode = StringToBool(parameters[Options.ExtraMode]);

            if (HasValue(parameters, Options.SafeMode))
                parser.SafeMode = StringToBool(parameters[Options.SafeMode]);

            if (HasValue(parameters, Options.MarkdownInHtml))
                parser.MarkdownInHtml = StringToBool(parameters[Options.MarkdownInHtml]);

            if (HasValue(parameters, Options.AutoHeadingIDs))
                parser.AutoHeadingIDs = StringToBool(parameters[Options.AutoHeadingIDs]);

            if (HasValue(parameters, Options.NewWindowForExternalLinks))
                parser.NewWindowForExternalLinks = StringToBool(parameters[Options.NewWindowForExternalLinks]);

            if (HasValue(parameters, Options.NewWindowForLocalLinks))
                parser.NewWindowForLocalLinks = StringToBool(parameters[Options.NewWindowForLocalLinks]);

            if (HasValue(parameters, Options.NoFollowLinks))
                parser.NoFollowLinks = StringToBool(parameters[Options.NoFollowLinks]);

            if (HasValue(parameters, Options.HtmlClassFootnotes))
                parser.HtmlClassFootnotes = parameters[Options.HtmlClassFootnotes];

            if (HasValue(parameters, Options.HtmlClassTitledImages))
                parser.HtmlClassTitledImages = parameters[Options.HtmlClassTitledImages];

            var output = parser.Transform(markdown);

            return output;
        }

        public static string BoolToString(bool? value)
        {
            switch (value)
            {
                case null:
                    return String.Empty;
                case true:
                    return "1";
                default:
                    return "0";
            }
        }

        public static bool StringToBool(string flag)
        {
            return flag == "1";
        }

        private static bool HasValue(SafeDictionary<string> list, string key)
        {
            if (list == null || key == null) return false;

            return !String.IsNullOrEmpty(list[key]);
        }
    }
}