using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Caching;
using Sitecore.SharedSource.Data;
using Sitecore.SharedSource.Extensions;

namespace Sitecore.SharedSource.Web
{
    public static class ResourceRenderer
    {
        private const string ScriptTagFormat = "<script src=\"{0}\"></script>";

        private const string StyleTagFormat = "<link href=\"{0}\" rel=\"stylesheet\" />";

        private static readonly string ScriptsFormat =
            Settings.GetSetting("CodeEditor.Media.ScriptsPath", "/Scripts/Media/") + "{0}{1}.{2}";

        private static readonly string StylesFormat =
            Settings.GetSetting("CodeEditor.Media.StylesPath", "/Content/Media/") + "{0}{1}.{2}";

        static ResourceRenderer()
        {
        }

        private static IHtmlString Render(ResourceType resourceType, params string[] paths)
        {
            var tagFormat = String.Empty;
            switch (resourceType)
            {
                case ResourceType.Script:
                    tagFormat = ScriptTagFormat;
                    break;
                case ResourceType.Style:
                    tagFormat = StyleTagFormat;
                    break;
            }
            return RenderFormat(tagFormat, paths);
        }

        private static IHtmlString RenderFormat(string tagFormat, params string[] paths)
        {
            Assert.ArgumentNotNullOrEmpty(tagFormat, "tagFormat");
            Assert.ArgumentNotNull(paths, "paths");

            foreach (var str in paths)
            {
                Assert.IsNotNullOrEmpty(str, "paths");
            }

            var stringBuilder = new StringBuilder();
            foreach (var path in paths)
            {
                stringBuilder.Append(String.Format(tagFormat, HttpUtility.UrlPathEncode(path)));
                stringBuilder.Append(Environment.NewLine);
            }
            return new HtmlString(stringBuilder.ToString());
        }

        private static IEnumerable<SaveResult> SaveItemsToFile(IEnumerable<string> ids)
        {
            var results = new List<SaveResult>();

            var suffix = HttpContext.Current.IsDebuggingEnabled ? String.Empty : ".min";

            foreach (var mediaItem in ids.Select(id => (MediaItem) Context.Database.GetItem(ID.Parse(id))))
            {
                switch (mediaItem.Extension.ToLower())
                {
                    case "js":
                        var scriptsRelativePath = String.Format(ScriptsFormat, mediaItem.Name, suffix,
                            mediaItem.Extension);
                        results.Add(mediaItem.ToFile(scriptsRelativePath,
                            HttpContext.Current.Server.MapPath(scriptsRelativePath), ResourceType.Script));
                        break;
                    case "css":
                        var stylesRelativePath = String.Format(StylesFormat, mediaItem.Name, suffix,
                            mediaItem.Extension);
                        results.Add(mediaItem.ToFile(stylesRelativePath,
                            HttpContext.Current.Server.MapPath(stylesRelativePath), ResourceType.Style));
                        break;
                }
            }

            return results;
        }

        /// <summary>
        ///     Renders the results into valid Html markup.
        /// </summary>
        /// <param name="results">The results to render.</param>
        /// <param name="resourceType">The type of resource to render.</param>
        /// <returns></returns>
        public static string RenderResourceHtmlString(List<SaveResult> results, ResourceType resourceType)
        {
            var content = String.Empty;
            switch (resourceType)
            {
                case ResourceType.Script:
                    content =
                        Render(resourceType,
                            results.Where(x => x.Resource == ResourceType.Script).Select(x => x.RelativePath).ToArray())
                            .ToHtmlString();
                    break;
                case ResourceType.Style:
                    content =
                        Render(resourceType,
                            results.Where(x => x.Resource == ResourceType.Style).Select(x => x.RelativePath).ToArray())
                            .ToHtmlString();
                    break;
            }

            return content;
        }

        public static List<SaveResult> LoadResourceReference(Item resource)
        {
            var cacheKey = resource.ID.ToShortID().ToString();
            if (!Context.PageMode.IsNormal)
            {
                // Clear the cache for each page request when the page is not viewed in the normal mode.
                CacheUtil.RemoveCachedItem(cacheKey);
            }

            var results = CacheUtil.GetCachedItem<List<SaveResult>>(cacheKey) ?? new List<SaveResult>();
            if (results.Any()) return results;

            if (String.IsNullOrEmpty(resource.Fields["Path"].Value)) return results;

            var ids = resource.Fields["Path"].Value.Split('|').ToList();
            results.AddRange(SaveItemsToFile(ids));

            CacheUtil.SetCachedItem(cacheKey, results);

            return results;
        }
    }
}