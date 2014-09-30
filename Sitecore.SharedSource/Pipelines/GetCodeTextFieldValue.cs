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
                args.Result.FirstPart = HtmlUtil.ReplaceLineBreaks(args.Result.FirstPart);
                args.Result.LastPart = HtmlUtil.ReplaceLineBreaks(args.Result.LastPart);
                return;
            }

            var parameters = args.GetField().Source.ToDictionary(args.Parameters);
            if (!parameters.ContainsKey("mode") || !parameters["mode"].Is("markdown")) return;

            var firstPart = HtmlUtil.ReplaceHtmlBreaks(args.Result.FirstPart);
            args.Result.FirstPart = MarkdownRenderer.Render(firstPart, parameters);

            var lastPart = HtmlUtil.ReplaceHtmlBreaks(args.Result.LastPart);
            args.Result.LastPart = MarkdownRenderer.Render(lastPart, parameters);
        }
    }
}