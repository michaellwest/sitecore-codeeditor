using System;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Sitecore.SharedSource.Extensions
{
    /// <summary>
    /// Represents a collection of Field extension methods.
    /// </summary>
    public static class FieldExtensions
    {
        /// <summary>
        /// Returns the datetime representation of the field.
        /// </summary>
        /// <param name="field">The date field.</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this Field field)
        {
            Assert.IsNotNull(field, "field");

            DateField dateField = field;
            return dateField == null ? default(DateTime) : DateUtil.IsoDateToDateTime(dateField.Value);
        }

        /// <summary>
        /// Returns the link representation of the field.
        /// </summary>
        /// <param name="field">The link field.</param>
        /// <returns></returns>
        public static string ToLink(this Field field)
        {
            Assert.IsNotNull(field, "field");

            LinkField lf = field;
            switch (lf.LinkType.ToLower())
            {
                case "internal":
                    // Use LinkMananger for internal links, if link is not empty
                    return lf.TargetItem != null ? LinkManager.GetItemUrl(lf.TargetItem) : String.Empty;
                case "media":
                    // Use MediaManager for media links, if link is not empty
                    return lf.TargetItem != null ? MediaManager.GetMediaUrl(lf.TargetItem) : String.Empty;
                case "anchor":
                    // Prefix anchor link with # if link if not empty
                    return !String.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : String.Empty;
                default:
                    // Return external links matching 'external'
                    // Return mailto link matching 'mailto'
                    // Return javascript matching 'javascript'
                    return lf.Url;
            }
        }

        /// <summary>
        /// Returns the url representation of the field.
        /// </summary>
        /// <param name="field">The </param>
        /// <returns></returns>
        public static string ToMediaUrl(this Field field)
        {
            Assert.IsNotNull(field, "field");

            var image = (ImageField) field;

            return image.MediaItem != null ? MediaManager.GetMediaUrl(image.MediaItem) : null;
        }
    }
}