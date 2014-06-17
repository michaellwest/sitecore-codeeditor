using System;
using System.IO;
using Sitecore.Diagnostics;

namespace Sitecore.SharedSource.IO
{
    public static class DirectoryUtil
    {
        /// <summary>
        /// Creates a directory structure reflecting the specified path.
        /// </summary>
        /// <param name="path">The path to the check.</param>
        /// <returns></returns>
        public static bool EnsureDirectoryExists(string path)
        {
            Assert.ArgumentNotNullOrEmpty(path, "path");
            Assert.AreEqual(-1, path.IndexOfAny(Path.GetInvalidPathChars()), "The specified path is invalid.");

            var folder = Path.GetDirectoryName(path);

            try
            {
                if (String.IsNullOrEmpty(folder))
                {
                    return false;
                }

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Log.Error(String.Format("Error creating the directory {0} due to unauthorized access.", folder), ex);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Error creating the directory {0}.", folder), ex);
                return false;
            }

            return true;
        }
    }
}