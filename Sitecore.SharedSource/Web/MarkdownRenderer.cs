using System;
using MarkdownDeep;
using Sitecore.Collections;
using Sitecore.SharedSource.Extensions;

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
            var parser = new Markdown
            {
                NewWindowForExternalLinks = true,
                HtmlClassFootnotes = String.Empty,
                HtmlClassTitledImages = String.Empty
            };

            // Default values

            // parses all html
            // parser.ExtraMode = true;
            // parser.SafeMode = true;
            // parser.MarkdownInHtml = true;
            // parser.NewWindowForLocalLinks = false;
            // parser.NoFollowLinks = false;
            // parser.AutoHeadingIDs = true;
            
            if (parameters.HasValue(Options.ExtraMode))
                parser.ExtraMode = parameters[Options.ExtraMode].ToBool();

            if (parameters.HasValue(Options.SafeMode))
                parser.SafeMode = parameters[Options.SafeMode].ToBool();

            if (parameters.HasValue(Options.MarkdownInHtml))
                parser.MarkdownInHtml = parameters[Options.MarkdownInHtml].ToBool();

            if (parameters.HasValue(Options.AutoHeadingIDs))
                parser.AutoHeadingIDs = parameters[Options.AutoHeadingIDs].ToBool();

            if (parameters.HasValue(Options.NewWindowForExternalLinks))
                parser.NewWindowForExternalLinks = parameters[Options.NewWindowForExternalLinks].ToBool();

            if (parameters.HasValue(Options.NewWindowForLocalLinks))
                parser.NewWindowForLocalLinks = parameters[Options.NewWindowForLocalLinks].ToBool();

            if (parameters.HasValue(Options.NoFollowLinks))
                parser.NoFollowLinks = parameters[Options.NoFollowLinks].ToBool();

            if (parameters.HasValue(Options.HtmlClassFootnotes))
                parser.HtmlClassFootnotes = parameters[Options.HtmlClassFootnotes];

            if (parameters.HasValue(Options.HtmlClassTitledImages))
                parser.HtmlClassTitledImages = parameters[Options.HtmlClassTitledImages];

            return parser.Transform(markdown);
        }
    }
}