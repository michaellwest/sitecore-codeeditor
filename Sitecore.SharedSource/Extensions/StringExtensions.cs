using System;
using System.Text.RegularExpressions;
using Sitecore.Collections;

namespace Sitecore.SharedSource.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns whether or not the strings are the same. A case-insensitive comparison is performed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="compare">The comparison value.</param>
        /// <returns>True if the values are the same.</returns>
        public static bool Is(this string value, string compare)
        {
            return String.Compare(value, compare, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static SafeDictionary<string> ToDictionary(this string source, SafeDictionary<string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new SafeDictionary<string>();
            }
            if (String.IsNullOrWhiteSpace(source)) return parameters;

            var keyPairs = source.Split('&');

            foreach (var kvp in keyPairs)
            {
                var keyValueArray = kvp.Split('=');
                var key = keyValueArray[0];
                var value = keyValueArray[1];

                if (String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value)) continue;

                if (parameters.ContainsKey(key))
                {
                    parameters[key] = value;
                }
                else
                {
                    parameters.Add(key, value);
                }
            }

            return parameters;
        }

        public static bool IsNumber(this string input)
        {
            return Regex.IsMatch(input, @"^\d+$", RegexOptions.Compiled);
        }

        public static int ToNumber(this string input, int defaultValue)
        {
            return IsNumber(input) ? int.Parse(input) : defaultValue;
        }
    }
}