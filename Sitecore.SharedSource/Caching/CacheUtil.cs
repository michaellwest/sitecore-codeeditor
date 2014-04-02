using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.Caching
{
    /// <summary>
    /// Represents a utility class for interacting with the Sitecore cache.
    /// </summary>
    public class CacheUtil
    {
        private static readonly object CacheLock = new object();

        private static readonly Dictionary<string, Cache> CacheCollection = new Dictionary<string, Cache>();

        private static Cache SitecoreCache(string site)
        {
            Cache cache;

            if (String.IsNullOrEmpty(site))
            {
                site = Context.Site == null ? "NoSite" : Context.Site.Name;
            }

            lock (CacheLock)
            {
                if (CacheCollection.ContainsKey(site))
                {
                    cache = CacheCollection[site];
                }
                else
                {
                    var cacheSize = StringUtil.ParseSizeString(Settings.GetSetting("Caching.CacheSize", "100MB"));
                    cache = Cache.GetNamedInstance(site, cacheSize);

                    CacheCollection.Add(site, cache);
                }
            }

            return cache;
        }

        /// <summary>
        /// Gets the item from the cache using the specified key.
        /// </summary>
        /// <typeparam name="T">The data type of the item to retrieve.</typeparam>
        /// <param name="cacheKey">The key to associate with the data.</param>
        /// <param name="site">The site to associate with the key.</param>
        /// <returns></returns>
        public static T GetCachedItem<T>(string cacheKey, string site = "")
        {
            if (String.IsNullOrEmpty(cacheKey))
            {
                return default(T);
            }

            var cachedItem = default(T);

            var key = cacheKey.ToLower();

            var cache = SitecoreCache(site);

            if (cache == null)
            {
                return cachedItem;
            }

            if (cache.ContainsKey(key))
            {
                cachedItem = (T) cache[key];
            }
            return cachedItem;
        }

        /// <summary>
        /// Sets the item into the cache using the specified key and data.
        /// </summary>
        /// <param name="cacheKey">The key associated with the data.</param>
        /// <param name="data">The data to cache.</param>
        /// <param name="site">The site associated with the key.</param>
        public static void SetCachedItem(string cacheKey, object data, string site = "")
        {
            if (String.IsNullOrEmpty(cacheKey) || data == null)
            {
                return;
            }

            var key = cacheKey.ToLower();

            var cache = SitecoreCache(site);
            if (cache == null)
            {
                return;
            }

            if (cache.ContainsKey(key))
            {
                cache.Remove(key);
            }

            try
            {
                // Not certain on how to obtain the exact size so the initial length is simply a buffer.
                long dataLength = 500;

                using (var ms = new MemoryStream())
                {
                    var binary = new BinaryFormatter();
                    binary.Serialize(ms, data);

                    dataLength += ms.Length;
                }

                cache.Add(key, data, dataLength);
            }
            catch (SerializationException ex)
            {
                Log.Error("An error was encountered while acquiring the length of the object for key: " + key, ex,
                    data.GetType());
            }
        }

        /// <summary>
        /// Removes the item from the cache using the specified key.
        /// </summary>
        /// <param name="cacheKey">The key associated with the data.</param>
        /// <param name="site">The site associated with the key.</param>
        /// <returns></returns>
        public static bool RemoveCachedItem(string cacheKey, string site = "")
        {
            if (String.IsNullOrEmpty(cacheKey))
            {
                return false;
            }

            var key = cacheKey.ToLower();

            var cache = SitecoreCache(site);

            if (cache != null)
            {
                if (cache.ContainsKey(key))
                {
                    cache.Remove(key);
                }
            }

            return true;
        }

        /// <summary>
        /// Removes all the items from the cache.
        /// </summary>
        /// <param name="site">The site associated with the cached items.</param>
        public static void ClearCache(string site = "")
        {
            var cache = SitecoreCache(site);

            if (cache == null)
            {
                return;
            }

            foreach (var key in cache.GetCacheKeys())
            {
                cache.Remove(key);
            }
        }
    }
}