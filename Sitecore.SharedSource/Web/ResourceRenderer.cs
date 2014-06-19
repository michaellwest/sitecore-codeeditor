using System;
using System.Collections.Generic;
using System.IO;
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
    public class ResourceRenderer : IDisposable
    {
        private const string ScriptTagFormat = "<script src='{0}'></script>";

        private const string StyleTagFormat = "<link href='{0}' rel='stylesheet' />";

        private static readonly string ScriptsFormat =
            Settings.GetSetting("CodeEditor.Media.ScriptsPath", "/Scripts/Media/") + "{0}{1}.{2}";

        private static readonly string StylesFormat =
            Settings.GetSetting("CodeEditor.Media.StylesPath", "/Content/Media/") + "{0}{1}.{2}";

        private static IHtmlString Render(ResourceType resourceType, params string[] paths)
        {
            Assert.ArgumentNotNull(paths, "paths");

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

            foreach (var item in ids.Select(id => Context.Database.GetItem(ID.Parse(id))).Where(item => item != null))
            {
                var mediaItem = (MediaItem) item;

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
        public string RenderResourceHtmlString(List<SaveResult> results, ResourceType resourceType)
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

        public List<SaveResult> LoadResourceReference(Item resource)
        {
            var cacheKey = resource.ID.ToShortID().ToString();
            if (!Context.PageMode.IsNormal)
            {
                // Clear the cache for each page request when the page is not viewed in the normal mode.
                CacheUtil.RemoveCachedItem(cacheKey);
            }

            var results = CacheUtil.GetCachedItem<List<SaveResult>>(cacheKey) ?? new List<SaveResult>();
            if (results.Any())
            {
                if (results.Any(result => !File.Exists(result.FilePath)))
                {
                    results.Clear();
                    CacheUtil.RemoveCachedItem(cacheKey);
                }
                else
                {
                    return results;
                }
            }

            var mediaPath = resource.Fields["MediaPath"];
            if (mediaPath == null || String.IsNullOrEmpty(mediaPath.Value)) return results;

            var ids = mediaPath.Value.Split('|').ToList();
            results.AddRange(SaveItemsToFile(ids));

            CacheUtil.SetCachedItem(cacheKey, results);

            return results;
        }

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
            }

            _disposed = true;
        }

        #endregion
    }
}