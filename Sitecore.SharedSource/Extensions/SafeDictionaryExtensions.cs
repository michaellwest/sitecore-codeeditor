using System;
using Sitecore.Collections;

namespace Sitecore.SharedSource.Extensions
{
    public static class SafeDictionaryExtensions
    {
        public static bool HasValue(this SafeDictionary<string> list, string key)
        {
            if (list == null || key == null) return false;

            return !String.IsNullOrEmpty(list[key]);
        }
    }
}