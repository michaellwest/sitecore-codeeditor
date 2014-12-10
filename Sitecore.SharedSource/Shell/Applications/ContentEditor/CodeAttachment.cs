using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
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
using HtmlUtil = Sitecore.SharedSource.Web.HtmlUtil;
using Version = Sitecore.Data.Version;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class CodeAttachment : Attachment
    {
        public CodeAttachment()
        {
            Class = "scContentControl";
            // Set minimum height so the control doesn't collapse when empty.
            Attributes.Add("style", "background-color: #fff; padding: 4px; min-height: 100px;");
        }

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
                case "contentattachment:editcode":
                    Sitecore.Context.ClientPage.Start(this, "EditCode");
                    return;
            }

            base.HandleMessage(message);
        }

        protected void EditCode(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var itemNotNull = Client.GetItemNotNull(Sitecore.Data.ID.Parse(ItemID), Language.Parse(ItemLanguage),
                Version.Parse(ItemVersion));
            MediaItem mediaItem = itemNotNull;
            if (!mediaItem.HasMediaStream(FieldID) && mediaItem.FilePath.Length == 0)
            {
                SheerResponse.Alert("No file has been attached.");
            }
            else
            {
                if (!SheerResponse.CheckModified())
                {
                    return;
                }

                var mediaUri = MediaUri.Parse(mediaItem);
                var media = MediaManager.GetMedia(mediaUri);
                Assert.IsNotNull(media, typeof (Media), "MediaUri: {0}", mediaUri);

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

        protected string RenderPreview(MediaItem mediaItem)
        {
            var content = String.Empty;
            if (mediaItem != null && MediaManager.HasMediaContent(mediaItem))
            {
                using (var mediaStream = mediaItem.GetMediaStream())
                {
                    using (var stream = new StreamReader(mediaStream))
                    {
                        content = stream.ReadToEnd();
                    }
                }
            }

            // Renders the html for the field preview in the content editor.
            return String.Format("<div style='height: 100%; overflow: auto; overflow-x: hidden;'>{0}</div>",
                HtmlUtil.ReplaceNewLines(HttpUtility.HtmlEncode(content)));
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            var mediaItem =
                (MediaItem) Client.ContentDatabase.GetItem(Sitecore.Data.ID.Parse(ItemID), Language.Parse(ItemLanguage),
                    Version.Parse(ItemVersion));

            if (mediaItem.InnerItem.TemplateID != TemplateIDs.UnversionedCode &&
                mediaItem.InnerItem.TemplateID != TemplateIDs.VersionedCode)
            {
                base.DoRender(output);
                return;
            }

            SetWidthAndHeightStyle();
            output.Write("<div {0}>{1}</div>", ControlAttributes, RenderPreview(mediaItem));
        }
    }
}