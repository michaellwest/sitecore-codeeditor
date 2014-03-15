using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Version = Sitecore.Data.Version;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class CodeAttachment : Attachment
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
                case "codeattachment:editcode":
                    Sitecore.Context.ClientPage.Start(this, "EditCode");
                    return;
            }

            base.HandleMessage(message);
        }

        protected void EditCode(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var itemNotNull = Client.GetItemNotNull(Data.ID.Parse(ItemID), Language.Parse(ItemLanguage),
                Version.Parse(ItemVersion));
            MediaItem mediaItem = itemNotNull;
            if (!mediaItem.HasMediaStream(FieldID) && mediaItem.FilePath.Length == 0)
            {
                SheerResponse.Alert("No file has been attached.", new string[0]);
            }
            else
            {
                if (!SheerResponse.CheckModified())
                {
                    return;
                }

                var mediaUri = MediaUri.Parse(mediaItem);
                var media = MediaManager.GetMedia(mediaUri);
                Assert.IsNotNull(media, typeof (Media), "MediaUri: {0}", new object[1]
                {
                    mediaUri
                });

                if (args.IsPostBack)
                {
                    if ((args.Result == null) || (args.Result == "undefined")) return;

                    var text = args.Result;
                    if (text == "__#!$No value$!#__")
                    {
                        text = string.Empty;
                    }

                    if (text == Value) return;
                    
                    var bytes = Encoding.UTF8.GetBytes(text);
                    using (var ms = new MemoryStream(bytes))
                    {
                        using (new EditContext(mediaItem, SecurityCheck.Disable))
                        {
                            using (var mediaStream = new MediaStream(ms, media.Extension, mediaItem))
                            {
                                media.SetStream(mediaStream);
                            }
                        }
                    }

                    Log.Audit(this, "Updated content for media item: {0}", AuditFormatter.FormatItem(mediaItem));

                    Sitecore.Context.ClientPage.SendMessage(this, "item:refresh(type=attachment)");
                }
                else
                {
                    var handle = new UrlHandle();
                    using (var mediaStream = media.GetStream())
                    {
                        using (var stream = new StreamReader(mediaStream.Stream))
                        {
                            handle["code"] = stream.ReadToEnd();
                        }
                    }

                    var urlString =
                        new UrlString(
                            "/sitecore/shell/~/xaml/Sitecore.SharedSource.Shell.Applications.ContentEditor.Dialogs.EditCode.aspx");

                    var parameters = new NameValueCollection();
                    switch (mediaItem.Extension.ToLower())
                    {
                        case "js":
                            parameters["mode"] = "javascript";
                            break;
                        case "css":
                            parameters["mode"] = "css";
                            break;
                        case "html":
                            parameters["mode"] = "html";
                            break;
                        case "xml":
                            parameters["mode"] = "xml";
                            break;
                        case "json":
                            parameters["mode"] = "json";
                            break;
                        case "txt":
                            parameters["mode"] = "text";
                            break;
                    }

                    if (parameters["mode"] != null)
                    {
                        urlString.Append("mode", parameters["mode"]);
                    }

                    handle.Add(urlString);
                    SheerResponse.ShowModalDialog(urlString.ToString(), "1000px", "500px", string.Empty, true);
                    args.WaitForPostBack();
                }
            }
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull(output, "output");
            var mediaItem =
                (MediaItem)
                    Client.ContentDatabase.GetItem(Data.ID.Parse(ItemID), Language.Parse(ItemLanguage),
                        Version.Parse(ItemVersion));

            var content = String.Empty;
            if (mediaItem != null && MediaManager.HasMediaContent(mediaItem))
            {
                using (var mediaStream = mediaItem.GetMediaStream())
                {
                    using (var stream = new StreamReader(mediaStream))
                    {
                        content = WebUtility.HtmlEncode(stream.ReadToEnd());
                    }
                }
            }

            output.Write("<div class=\"scContentControl\" style=\"background-color: #fff;font: 13pt tahoma;padding: 4px;height: 150px;overflow: hidden;\">");
            output.Write(content);
            output.Write("</div>");
        }
    }
}