using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.SharedSource.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Replaces all occurrences of a specified System.String in this instance
        /// with another specified System.String. 
        /// </summary>
        /// <param name="original">The System.String object to be searched.</param>
        /// <param name="oldValue">A System.String to be replaced.</param>
        /// <param name="newValue">A System.String to replace all occurrences of oldValue.</param>
        /// <param name="comparisonType">One of the System.StringComparison values that
        /// determines how this string and value are compared.</param>
        /// <returns>A System.String equivalent to this instance but with all
        /// instances of oldValue replaced with newValue.</returns>
        /// <exception cref="System.ArgumentNullException">oldValue is null.</exception>
        /// <exception cref="System.ArgumentException">oldValue is the empty string ("").</exception>
        public static string Replace(this string original, string oldValue, string newValue,
            StringComparison comparisonType)
        {
            if (original == null)
            {
                throw new NullReferenceException("Object reference not set to an instance of an object.");
            }

            if (oldValue == null)
            {
                throw new ArgumentNullException("oldValue");
            }

            if (String.Empty == oldValue)
            {
                throw new ArgumentException("oldValue");
            }

            var lenPattern = oldValue.Length;
            var idxPattern = -1;
            var idxLast = 0;

            var result = new StringBuilder(original.Length);

            while (true)
            {
                idxPattern = original.IndexOf(oldValue, idxPattern + 1, comparisonType);

                if (idxPattern < 0)
                {
                    result.Append(original, idxLast, original.Length - idxLast);
                    break;
                }

                result.Append(original, idxLast, idxPattern - idxLast);
                result.Append(newValue);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }

        public static string Replace(this string value, string[] values, Func<string, string> formatter,
            StringComparison comparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            var sb = new StringBuilder(value.Length);

            int index;
            var lastEndIndex = 0;
            string def;
            while ((index = IndexOf(value, values, lastEndIndex, comparisonType, out def)) != -1)
            {
                sb.Append(value.Substring(lastEndIndex, index - lastEndIndex));
                sb.Append(formatter(def));
                lastEndIndex = index + def.Length;
            }
            sb.Append(value.Substring(lastEndIndex, value.Length - lastEndIndex));

            return sb.ToString();
        }

        private static int IndexOf(string text, IEnumerable<string> values, int startIndex,
            StringComparison comparisonType, out string foundEntry)
        {
            var minEntry = String.Empty;
            int[] minIndex = {-1};
            var index = -1;
            foreach (
                var entry in
                    values.Where(
                        value =>
                            ((index = text.IndexOf(value, startIndex, comparisonType)) < minIndex[0] && index != -1) ||
                            minIndex[0] == -1))
            {
                minIndex[0] = index;
                if (index > -1 && entry.Length > 0)
                {
                    minEntry = text.Substring(index, entry.Length); //entry;
                }
            }

            foundEntry = minEntry;
            return minIndex[0];
        }

        /// <summary>
        /// Creates a snippet of the specified text using the provided keywords and length. The maximum length of text before the keyword is 50% of the maxLength.
        /// </summary>
        /// <param name="value">The original body of text.</param>
        /// <param name="keywords">The keywords to match.</param>
        /// <param name="maxLength">The maximum length of the snippet.</param>
        /// <param name="highlight"></param>
        /// <returns></returns>
        public static string ToSnippet(this string value, string[] keywords, int maxLength, bool highlight = true)
        {
            if (String.IsNullOrEmpty(value) || keywords == null)
            {
                return String.Empty;
            }

            var snippedString = new StringBuilder();
            var keywordLocations = new List<int>();

            //Get the locations of all keywords
            for (var i = 0; i < keywords.Count(); i++)
            {
                keywordLocations.AddRange(IndexOfAll(value, keywords[i], StringComparison.CurrentCultureIgnoreCase));
            }

            //Sort locations
            keywordLocations.Sort();

            //Remove locations which are closer to each other than the SnippetLength
            if (keywordLocations.Count > 1)
            {
                var found = true;
                while (found)
                {
                    found = false;
                    for (var i = keywordLocations.Count - 1; i > 0; i--)
                    {
                        if (keywordLocations[i] - keywordLocations[i - 1] >= maxLength/2)
                        {
                            continue;
                        }

                        keywordLocations[i - 1] = (keywordLocations[i] + keywordLocations[i - 1])/2;
                        keywordLocations.RemoveAt(i);
                        found = true;
                    }
                }
            }

            //Make the snippets
            if (keywordLocations.Count > 0 && keywordLocations[0] - maxLength/2 > 0)
            {
                snippedString.Append("... ");
            }
            foreach (var i in keywordLocations)
            {
                var stringStart = Math.Max(0, i - maxLength/2);
                var stringEnd = Math.Min(i + maxLength/2, value.Length);
                var stringLength = Math.Min(stringEnd - stringStart, value.Length - stringStart);
                snippedString.Append(value.Substring(stringStart, stringLength));
                if (stringEnd < value.Length)
                {
                    snippedString.Append(" ... ");
                }
                if (snippedString.Length > 200)
                {
                    break;
                }
            }

            return highlight ? snippedString.ToString().WithHighlightedWords(keywords) : snippedString.ToString();
        }

        public static string WithHighlightedWords(this string value, string[] words)
        {
            if (!String.IsNullOrEmpty(value) && words != null)
            {
                return value.Replace(words, current => String.Format("<b>{0}</b>", current));
            }
            return value;
        }

        private static IEnumerable<int> IndexOfAll(string haystack, string needle, StringComparison comparison)
        {
            int pos;
            var offset = 0;
            var length = needle.Length;
            var positions = new List<int>();
            while ((pos = haystack.IndexOf(needle, offset, comparison)) != -1)
            {
                positions.Add(pos);
                offset = pos + length;
            }
            return positions;
        }

        /// <summary>
        /// Returns whether or not the list of values are contained in the source. A case-insensitive search is performed
        /// when the System.Type of value is System.String.
        /// </summary>
        /// <typeparam name="T">The type of value to find.</typeparam>
        /// <param name="value">The value to find.</param>
        /// <param name="list">The collection of items to determine if the value exists.</param>
        /// <returns>True if the value exists in the list.</returns>
        public static bool In<T>(this T value, params T[] list)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (list == null)
            {
                return false;
            }

            if (Type.GetTypeCode(typeof (T)) == TypeCode.String)
            {
                var items = new List<T>(list);
                var items1 = items.ConvertAll(item => (string) System.Convert.ChangeType(item, TypeCode.String));
                return items1.Contains((string) System.Convert.ChangeType(value, TypeCode.String),
                    StringComparer.OrdinalIgnoreCase);
            }
            return list.Contains(value);
        }

        /// <summary>
        /// Returns whether or not the strings are the same. A case-insensitive comparison is performed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="compare">The comparison value.</param>
        /// <returns>True if the values are the same.</returns>
        public static bool Is(this string value, string compare)
        {
            return String.Compare(value, compare, true) == 0;
        }
    }
}