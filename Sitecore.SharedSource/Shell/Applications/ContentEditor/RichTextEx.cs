using System;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Configuration;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class RichTextEx : RichText
    {
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if (message["id"] != ID) return;

            string messageText;
            if ((messageText = message.Name) == null)
            {
                return;
            }

            switch (messageText)
            {
                case "richtext:edithtml":
                    Sitecore.Context.ClientPage.Start(this, "EditHtmlEx");
                    return;
            }

            base.HandleMessage(message);
        }

        protected void EditHtmlEx(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Disabled) return;

            if (args.IsPostBack)
            {
                if ((args.Result != null) && (args.Result != "undefined"))
                {
                    UpdateHtml(args);
                }
            }
            else
            {
                var urlString =
                    new UrlString(
                        "/sitecore/shell/~/xaml/Sitecore.SharedSource.Shell.Applications.ContentEditor.Dialogs.EditCode.aspx");

                var settings = ApplicationSettings.GetDialogSettings();

                var parameters = WebUtil.ParseUrlParameters(Source);
                if (parameters["mode"] != null)
                {
                    urlString.Append("mode", parameters["mode"]);
                }
                urlString.Append("theme", parameters["theme"] ?? settings.Theme);
                urlString.Append("fontSize", parameters["fontSize"] ?? settings.FontSize.ToString());
                urlString.Append("fontFamily", parameters["fontFamily"] ?? settings.FontFamily);

                var handle = new UrlHandle();
                var str2 = Value;
                if (str2 == "__#!$No value$!#__")
                {
                    str2 = string.Empty;
                }
                handle["code"] = str2;
                handle.Add(urlString);

                SheerResponse.ShowModalDialog(urlString.ToString(), settings.Width + "px", settings.Height + "px",
                    String.Empty, true);
                args.WaitForPostBack();
            }
        }
    }
}