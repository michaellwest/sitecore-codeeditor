using System.Linq;
using System.Web.UI;
using Sitecore.SharedSource.Data;
using Sitecore.Web.UI;

namespace Sitecore.SharedSource.Web.UI.WebControls
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

            // Determine if the page has a resource.
            var pageItem = Sitecore.Context.Item;
            var resource = pageItem.Template.BaseTemplates.Any(x => x.ID == TemplateIDs.MediaResource) ? pageItem : null;
            if (resource == null)
            {
                return;
            }

            using (var renderer = new ResourceRenderer())
            {
                var results = renderer.LoadResourceReference(resource);

                if (ResourceType == ResourceType.Script && results.Any(x => x.Resource == ResourceType.Script))
                {
                    writer.Write(renderer.RenderResourceHtmlString(results, ResourceType.Script));
                }
                else if (ResourceType == ResourceType.Style && results.Any(x => x.Resource == ResourceType.Style))
                {
                    writer.Write(renderer.RenderResourceHtmlString(results, ResourceType.Style));
                }
            }
        }

        protected override string GetCachingID()
        {
            return GetType().FullName + "_" + UniqueID;
        }
    }
}