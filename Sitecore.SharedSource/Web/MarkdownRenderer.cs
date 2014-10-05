using MarkdownDeep;
using Sitecore.Collections;
using Sitecore.SharedSource.Extensions;
using System;
using System.Linq;

namespace Sitecore.SharedSource.Web
{
    public class MarkdownRenderer
    {
        public static string Render(string markdown, SafeDictionary<string> parameters)
        {
            if (String.IsNullOrEmpty(markdown)) return markdown;

            var parser = new Markdown
            {
                NewWindowForExternalLinks = true,
                HtmlClassFootnotes = String.Empty,
                HtmlClassTitledImages = String.Empty
            };

            if (parameters.Any())
            {
                var properties = parser.GetPropertiesDictionary();

                foreach (var key in parameters.Keys)
                {
                    if (!properties.ContainsKey(key)) continue;

                    var property = properties.SingleOrDefault(p => p.Key.Is(key));

                    parser.SetPropertyValue(property.Key, parameters[key]);
                }
            }

            return parser.Transform(markdown);
        }
    }
}