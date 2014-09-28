using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.SharedSource.Extensions;
using Sitecore.SharedSource.Web;
using System;
using System.Text.RegularExpressions;

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
                args.Result.FirstPart = ReplaceLineBreaks(args.Result.FirstPart);
                args.Result.LastPart = ReplaceLineBreaks(args.Result.LastPart);
                return;
            }

            var parameters = args.GetField().Source.ToDictionary(args.Parameters);
            if (!parameters.ContainsKey("mode") || !parameters["mode"].Is("markdown")) return;

            var firstPart = ReplaceHtmlBreaks(args.Result.FirstPart);
            args.Result.FirstPart = MarkdownRenderer.Render(firstPart, parameters);

            var lastPart = ReplaceHtmlBreaks(args.Result.LastPart);
            args.Result.LastPart = MarkdownRenderer.Render(lastPart, parameters);
        }

        private static string ReplaceLineBreaks(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"(\s{2,}\n)(\r\n|\n)", "<br />", RegexOptions.Compiled);
        }

        private static string ReplaceHtmlBreaks(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;

            return Regex.Replace(input, @"(<br>)|(<br */>)|(\[br */\])", "  \n", RegexOptions.Compiled);
        }
    }
}