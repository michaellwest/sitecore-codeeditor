using Sitecore.Configuration;
using Sitecore.SharedSource.Extensions;

namespace Sitecore.SharedSource.Configuration
{
    public sealed class ApplicationSettings
    {
        /// <summary>
        /// Gets the configured dialog settings.
        /// </summary>
        /// <returns></returns>
        public static DialogSettings GetDialogSettings()
        {
            var settingsDb = Factory.GetDatabase(Settings.GetSetting("CodeEditor.Settings.Database", "master"));

            var allusers = settingsDb.GetItem(ItemIDs.AllUsers);

            var settings = new DialogSettings
            {
                Theme = allusers.Fields["Theme"].Value.ToLower(),
                Width = allusers.Fields["Width"].Value.ToNumber(1000),
                Height = allusers.Fields["Height"].Value.ToNumber(500),
                FontSize = allusers.Fields["FontSize"].Value.ToNumber(12),
                FontFamily = allusers.Fields["FontFamily"].Value
            };

            return settings;
        }
    }
}