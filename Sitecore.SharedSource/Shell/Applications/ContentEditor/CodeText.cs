using System;
using System.Web;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Configuration;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using HtmlUtil = Sitecore.SharedSource.Web.HtmlUtil;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class CodeText : Memo
    {
        public string ItemLanguage
        {
            get { return StringUtil.GetString(ServerProperties["ItemLanguage"]); }
            set { ServerProperties["ItemLanguage"] = value; }
        }

        public string Source
        {
            get { return GetViewStateString("Source"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Source", value);
            }
        }

        public override string Value
        {
            get { return GetViewStateString("Value"); }
            set
            {
                if (value == Value) return;

                SetViewStateString("Value", value);

                // Encode so the content editor will properly render the content.
                SheerResponse.SetInnerHtml(ID, RenderPreview());
            }
        }

        public CodeText()
        {
            Class = "scContentControl";
            // Set minimum height so the control doesn't collapse when empty.
            Attributes.Add("style", "background-color: #fff; padding: 4px; min-height: 100px;");
        }

        protected override void OnPreRender(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");

            base.OnPreRender(e);
            ServerProperties["Value"] = ServerProperties["Value"];
            Disabled = false;
        }

        protected void UpdateText(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            var text = args.Result;
            if (text == "__#!$No value$!#__")
            {
                text = string.Empty;
            }

            if (text == Value) return;

            SetModified();

            Value = text;
        }

        protected void EditCode(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (Disabled) return;

            if (args.IsPostBack)
            {
                if ((args.Result != null) && (args.Result != "undefined"))
                {
                    UpdateText(args);
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

        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if (message["id"] != ID) return;

            string messageText;
            if ((messageText = message.Name) == null) return;

            switch (messageText)
            {
                case "codetext:editcode":
                    Sitecore.Context.ClientPage.Start(this, "EditCode");
                    return;
            }

            base.HandleMessage(message);
        }

        protected string RenderPreview()
        {
            // Renders the html for the field preview in the content editor.
            return String.Format("<div style='height: 100%; overflow: auto; overflow-x: hidden;'>{0}</div>",
                HtmlUtil.ReplaceNewLines(HttpUtility.HtmlEncode(Value)));
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            SetWidthAndHeightStyle();
            output.Write("<div {0}>{1}</div>", ControlAttributes, RenderPreview());
        }
    }
}