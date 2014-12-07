using System;
using System.IO;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentManager;

namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor
{
    public class EditorFormatter : Sitecore.Shell.Applications.ContentEditor.EditorFormatter
    {
        public override void RenderMenuButtons(Control parent, Editor.Field field, Item fieldType, bool readOnly)
        {
            if (fieldType.ID != SharedSource.TemplateIDs.Attachment)
            {
                base.RenderMenuButtons(parent, field, fieldType, readOnly);
                return;
            }

            var fieldTemplateId = field.TemplateField.Template.ID;
            if (fieldTemplateId != SharedSource.TemplateIDs.UnversionedCode &&
                fieldTemplateId != SharedSource.TemplateIDs.UnversionedCode)
            {
                base.RenderMenuButtons(parent, field, fieldType, readOnly);
                return;
            }

            var writer = new HtmlTextWriter(new StringWriter());
            writer.Write("<div class=\"scContentButtons\">");
            writer.Write(GetDownloadLink(field.ControlID, "Download"));
            writer.Write("&#183;");
            writer.Write(GetEditCodeLink(field.ControlID, "Edit Code"));
            writer.Write("</div>");
            AddLiteralControl(parent, writer.InnerWriter.ToString());
        }

        private string GetDownloadLink(string id, string text)
        {
            return String.Format(
                "<a href=\"#\" class=\"scContentButton\" onclick=\"{0}\">{1}</a>",
                Context.ClientPage.GetClientEvent("contentattachment:download(id=" + id + ")"),
                Translate.Text(text));
        }

        private string GetEditCodeLink(string id, string text)
        {
            return String.Format(
                "<a href=\"#\" class=\"scContentButton\" onclick=\"{0}\">{1}</a>",
                Context.ClientPage.GetClientEvent("contentattachment:editcode(id=" + id + ")"),
                Translate.Text(text));
        }
    }
}