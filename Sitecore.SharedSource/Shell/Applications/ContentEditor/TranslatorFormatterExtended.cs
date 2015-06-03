using System;
using System.IO;
using System.Web.UI;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentManager;

namespace Sitecore.SharedSource.Shell.Applications.ContentEditor
{
    public class TranslatorFormatterExtended : TranslatorFormatter
    {
        public override void RenderMenuButtons(Control parent, Editor.Field field, Item fieldType, bool readOnly)
        {
            if (fieldType.ID != TemplateIDs.Attachment)
            {
                base.RenderMenuButtons(parent, field, fieldType, readOnly);
                return;
            }

            var fieldTemplateId = field.TemplateField.Template.ID;
            if (fieldTemplateId != TemplateIDs.UnversionedCode &&
                fieldTemplateId != TemplateIDs.VersionedCode)
            {
                base.RenderMenuButtons(parent, field, fieldType, readOnly);
                return;
            }

            var writer = new HtmlTextWriter(new StringWriter());
            writer.Write("<div class='scContentButtons'>");
            writer.Write(GetLink(field.ControlID, "Download", "download"));
            writer.Write("&#183;");
            writer.Write(GetLink(field.ControlID, "Edit Code", "editcode"));
            writer.Write("</div>");
            AddLiteralControl(parent, writer.InnerWriter.ToString());
        }

        private static string GetLink(string id, string text, string message)
        {
            var sheerEvent = Context.ClientPage.GetClientEvent(String.Format("content:{0}(id={1})", message, id));
            return String.Format("<a href=\"#\" class=\"scContentButton\" onclick=\"{0}\">{1}</a>", sheerEvent, Translate.Text(text));
        }
    }
}