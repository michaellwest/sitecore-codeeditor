using System;

namespace Sitecore.SharedSource.Data
{
    [Serializable]
    public class SaveResult
    {
        /// <summary>
        /// Gets or sets the relative server path to the file.
        /// </summary>
        public string RelativePath { get; set; }

        /// <summary>
        /// Gets or sets the file path local to the server.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the type of resource saved.
        /// </summary>
        public ResourceType Resource { get; set; }
    }
}