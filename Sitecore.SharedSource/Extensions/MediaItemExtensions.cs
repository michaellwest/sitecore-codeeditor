using System;
using System.IO;
using System.Text;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources.Media;
using Sitecore.SharedSource.Data;
using Sitecore.SharedSource.IO;
using Sitecore.SharedSource.Microsoft.Ajax.Utilities;
using Sitecore.SharedSource.Microsoft.Ajax.Utilities.Css;
using Sitecore.SharedSource.Microsoft.Ajax.Utilities.JavaScript;

namespace Sitecore.SharedSource.Extensions
{
    /// <summary>
    /// Represents a collection of MediaItem extension methods.
    /// </summary>
    public static class MediaItemExtensions
    {
        /// <summary>
        /// Saves a media item to file at the specified relative path.
        /// </summary>
        /// <param name="mediaItem">The media item to save.</param>
        /// <param name="relativePath">The relative server path to the file.</param>
        /// <param name="filePath">The path to save the file.</param>
        /// <param name="resourceType">The type of resource.</param>
        /// <returns>The relative path to the file.</returns>
        /// <remarks>Adapted from the article http://learningsitecore.com/2013/07/17/save-all-images-of-a-media-library-folder-to-disk-2/ </remarks>
        public static SaveResult ToFile(this MediaItem mediaItem, string relativePath, string filePath,
            ResourceType resourceType)
        {
            if (!DirectoryUtil.EnsureDirectoryExists(filePath))
            {
                return new SaveResult();
            }

            // Only write the file if it does not yet exist or if the database contains a newer version.
            var updated = mediaItem.InnerItem.Statistics.Updated;
            if (File.Exists(filePath))
            {
                try
                {
                    var lastWrite = File.GetLastWriteTime(filePath);
                    if (lastWrite > updated)
                    {
                        if (!HttpContext.Current.IsDebuggingEnabled)
                        {
                            relativePath += "?v=" + FileUtil.ComputeSha256UrlEncodedHash(filePath);
                        }

                        Log.Info(
                            String.Format(
                                "Skipped saving the media item {0} with id {1} to the specified path {2} because it has not changed.",
                                mediaItem.Name, mediaItem.ID, filePath), mediaItem.GetType());
                        return new SaveResult {Exists = true, RelativePath = relativePath, Resource = resourceType};
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Log.Error(
                        String.Format("Error saving the media item {0} with id {1} to the specified path {2}.",
                            mediaItem.Name, mediaItem.ID, filePath), ex, mediaItem.GetType());
                    return new SaveResult();
                }
            }

            var media = MediaManager.GetMedia(mediaItem);
            using (var source = media.GetStream())
            {
                try
                {
                    // Check if the debug attribute is configured to true.
                    if (HttpContext.Current.IsDebuggingEnabled)
                    {
                        using (var destination = File.OpenWrite(filePath))
                        {
                            source.CopyTo(destination);
                            destination.Flush();
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(source.Stream, Encoding.UTF8))
                        {
                            var contents = MinifyContent(reader.ReadToEnd(), resourceType);
                            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
                            {
                                using (var destination = File.Create(filePath))
                                {
                                    ms.CopyTo(destination);
                                    destination.Flush();
                                }
                            }
                        }
                        relativePath += "?v=" + FileUtil.ComputeSha256UrlEncodedHash(filePath);
                    }

                    Log.Info(
                        String.Format("Completed saving the media item {0} with id {1} to the specified path {2}.",
                            mediaItem.Name, mediaItem.ID, filePath), mediaItem.GetType());
                }
                catch (Exception ex)
                {
                    Log.Error(
                        String.Format("Error saving the media item {0} with id {1} to the specified path {2}.",
                            mediaItem.Name, mediaItem.ID, filePath), ex, mediaItem.GetType());
                    return new SaveResult {WasSaved = false};
                }

                return new SaveResult
                {
                    Exists = true,
                    WasSaved = true,
                    RelativePath = relativePath,
                    Resource = resourceType
                };
            }
        }

        /// <summary>
        /// Returns a minified version of the specified content.
        /// </summary>
        /// <param name="content">The content to minify.</param>
        /// <param name="resourceType">The type of content.</param>
        /// <returns></returns>
        public static string MinifyContent(string content, ResourceType resourceType)
        {
            var result = content;

            var minifier = new Minifier();
            switch (resourceType)
            {
                case ResourceType.Script:
                    var jsSettings = new CodeSettings
                    {
                        EvalTreatment = EvalTreatment.MakeImmediateSafe,
                        PreserveImportantComments = false
                    };
                    var js = minifier.MinifyJavaScript(content, jsSettings);
                    if (minifier.ErrorList.Count == 0)
                    {
                        result = js;
                    }
                    break;
                case ResourceType.Style:
                    var cssSettings = new CssSettings {CommentMode = CssComment.None};
                    var css = minifier.MinifyStyleSheet(content, cssSettings);
                    if (minifier.ErrorList.Count == 0)
                    {
                        result = css;
                    }
                    break;
            }

            return result;
        }
    }
}