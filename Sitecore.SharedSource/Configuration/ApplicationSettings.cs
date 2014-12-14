using Sitecore.Configuration;
using Sitecore.Data;
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

            // /sitecore/system/Modules/Code Editor/Settings/Dialog/All Users
            var allusersId = ID.Parse("{9DAC8FA4-EE28-4D1F-842E-C107F51E2E6C}");
            var allusers = settingsDb.GetItem(allusersId);

            var width = Settings.GetSetting("CodeEditor.Dialog.Width").ToNumber(1000);
            var height = Settings.GetSetting("CodeEditor.Dialog.Height").ToNumber(500);

            var settings = new DialogSettings
            {
                Theme = allusers.Fields["Theme"].Value.ToLower(),
                Width = allusers.Fields["Width"].Value.ToNumber(width),
                Height = allusers.Fields["Height"].Value.ToNumber(height),
                FontSize = allusers.Fields["FontSize"].Value.ToNumber(12),
                FontFamily = allusers.Fields["FontFamily"].Value
            };

            return settings;
        }
    }
}