using Sitecore.Pipelines.RenderField;
using Sitecore.SharedSource.Extensions;
using Sitecore.SharedSource.Web;

namespace Sitecore.SharedSource.Pipelines
{
    public class GetCodeTextFieldValue
    {
        public void Process(RenderFieldArgs args)
        {
            var fieldTypeKey = args.FieldTypeKey;
            if (fieldTypeKey != "code text") return;

            var parameters = args.GetField().Source.ToDictionary(args.Parameters);
            if (!parameters.ContainsKey("mode") || !parameters["mode"].Is("markdown")) return;

            var rawMarkdown = args.Result.FirstPart;
            args.Result.FirstPart = MarkdownRenderer.Render(rawMarkdown, parameters);
        }
    }
}