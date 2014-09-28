using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sitecore.SharedSource.Extensions
{
    public static class ReflectionExtensions
    {
        public static Dictionary<string, string> GetPropertiesDictionary<T>(this T item)
        {
            return GetPropertiesDictionary(item, StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, string> GetPropertiesDictionary<T>(this T item, StringComparer comparer)
        {
            if (item == null) return null;

            return item.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => (p.GetValue(item, null) ?? String.Empty).ToString(), comparer);
        }

        public static void SetPropertyValue<T>(this T item, string propertyName, object value)
        {
            if (item == null) return;

            var property = item.GetType().GetProperty(propertyName);
            property.SetValue(item, System.Convert.ChangeType(value, property.PropertyType), null);
        }
    }
}