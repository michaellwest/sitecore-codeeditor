using System.IO;
using System.Security.Cryptography;
using System.Web;

namespace Sitecore.SharedSource.IO
{
    public static class FileUtil
    {
        /// <summary>
        /// Generates a SHA256 hash for the specified file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns></returns>
        public static string ComputeSha256UrlEncodedHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    return HttpServerUtility.UrlTokenEncode(hash);
                }
            }
        }
    }
}