using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.SharedSource.Extensions;

namespace Sitecore.SharedSource.Configuration
{
    public sealed class ApplicationSettings
    {
        public static DialogSettings GetDialogSettings()
        {
            // /sitecore/system/Modules/Code Editor/Settings/Dialog/All Users
            var allusers = Factory.GetDatabase("master").GetItem(ID.Parse("{9DAC8FA4-EE28-4D1F-842E-C107F51E2E6C}"));

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