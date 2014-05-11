using System.Linq;
using System.Web.UI;
using Sitecore.SharedSource.Data;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.SharedSource.Web.UI
{
    [ToolboxData("<{0}:ResourceWebControl runat=server></{0}:ResourceWebControl>")]
    public class ResourceWebControl : WebControl
    {
        public ResourceType ResourceType { get; set; }

        protected override void DoRender(HtmlTextWriter writer)
        {
            if (Sitecore.Context.Item == null || Sitecore.Context.Database == null)
            {
                return;
            }

            // Determine if the page has a resources.
            var pageItem = Sitecore.Context.Item;
            var resource = pageItem.Children.FirstOrDefault(x => x.TemplateID == TemplateIDs.ResourceReference);
            if (resource == null)
            {
                return;
            }

            var results = ResourceRenderer.LoadResourceReference(resource);

            if (ResourceType == ResourceType.Script && results.Any(x => x.Resource == ResourceType.Script))
            {
                writer.Write(ResourceRenderer.RenderResourceHtmlString(results, ResourceType.Script));
            }
            else if (ResourceType == ResourceType.Style && results.Any(x => x.Resource == ResourceType.Style))
            {
                writer.Write(ResourceRenderer.RenderResourceHtmlString(results, ResourceType.Style));
            }
        }
    }
}