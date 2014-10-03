using System.Web;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.SharedSource.Data;
using Sitecore.SharedSource.Extensions;
using Sitecore.SharedSource.Web;

namespace Sitecore.SharedSource.Pipelines
{
    public class GetCodeTextFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var fieldTypeKey = args.FieldTypeKey;
            if (fieldTypeKey != "code text") return;

            if (Context.PageMode.IsPageEditorEditing)
            {
                // Encode so the page editor will render the html.
                // Replace line breaks so the spacing is correct.
                args.Result.FirstPart = HtmlUtil.ReplaceNewLines(HttpUtility.HtmlEncode(args.Result.FirstPart));
                args.Result.LastPart = HtmlUtil.ReplaceNewLines(HttpUtility.HtmlEncode(args.Result.LastPart));
                return;
            }

            var parameters = args.GetField().Source.ToDictionary(args.Parameters);
            if (!parameters.ContainsKey("mode") || !parameters["mode"].Is("markdown")) return;

            // Decode the html and then convert new lines to html breaks.
            var firstPart = args.Result.FirstPart;//HtmlUtil.ReplaceNewLines(HttpUtility.HtmlDecode(args.Result.FirstPart));
            args.Result.FirstPart = MarkdownRenderer.Render(firstPart, parameters);

            var lastPart = HtmlUtil.ReplaceNewLines(HtmlUtil.ReplaceLineBreaks(args.Result.LastPart));
            args.Result.LastPart = MarkdownRenderer.Render(lastPart, parameters);
        }
    }
}