using System.Web;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using HtmlUtil = Sitecore.SharedSource.Data.HtmlUtil;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class CodeText : Memo
    {
        public CodeText()
        {
            this.Class = "scContentControl";
            this.Attributes.Add("style", "background-color: #fff; padding: 4px; min-height: 100px;");
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
                case "codetext:editcode":
                    Sitecore.Context.ClientPage.Start(this, "EditCode");
                    return;
            }

            base.HandleMessage(message);
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
                var parameters = WebUtil.ParseUrlParameters(Source);
                if (parameters["mode"] != null)
                {
                    urlString.Append("mode", parameters["mode"]);
                }
                if (parameters["theme"] != null)
                {
                    urlString.Append("theme", parameters["theme"]);
                }
                var handle = new UrlHandle();
                var str2 = Value;
                if (str2 == "__#!$No value$!#__")
                {
                    str2 = string.Empty;
                }
                handle["code"] = str2;
                handle.Add(urlString);
                SheerResponse.ShowModalDialog(urlString.ToString(), "1000px", "500px", string.Empty, true);
                args.WaitForPostBack();
            }
        }

        /// <summary>
        /// Updates the Text. 
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected virtual void UpdateText(ClientPipelineArgs args)
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

        /// <summary>
        /// Gets or sets the source.
        /// 
        /// </summary>
        /// 
        /// <value>
        /// The source.
        /// </value>
        public string Source
        {
            get { return this.GetViewStateString("Source"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.SetViewStateString("Source", value);
            }
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            this.SetWidthAndHeightStyle();
            output.Write("<div " + this.ControlAttributes + "><div style='height: 100%; overflow: hidden;'>" +
                         HtmlUtil.ReplaceLineBreaks(HttpUtility.HtmlEncode(this.Value)));
            this.RenderChildren(output);
            output.Write("</div></div>");
        }

        public override string Value
        {
            get
            {
                return this.GetViewStateString("Value");
            }
            set
            {
                if (!(value != this.Value))
                    return;
                this.SetViewStateString("Value", value);

                // Encode so the content editor will properly render the content.
                SheerResponse.SetInnerHtml(this.ID, HttpUtility.HtmlEncode(value));
            }
        }
    }
}