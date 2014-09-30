using Sitecore.ContentSearch;
using Sitecore.ContentSearch.FieldReaders;
using Sitecore.Data.Fields;
using Sitecore.SharedSource.Extensions;
using Sitecore.SharedSource.Web;

namespace Sitecore.SharedSource.Data.Fields
{
    public class CodeTextFieldReader : FieldReader
    {
        public override object GetFieldValue(IIndexableDataField indexableField)
        {
            var field = (Field) (indexableField as SitecoreItemDataField);

            var parameters = field.Source.ToDictionary();
            if (!parameters.ContainsKey("mode") || !parameters["mode"].Is("markdown")) return field.Value;

            var renderedMarkdown = MarkdownRenderer.Render(field.Value, parameters);

            return renderedMarkdown;
        }
    }
}