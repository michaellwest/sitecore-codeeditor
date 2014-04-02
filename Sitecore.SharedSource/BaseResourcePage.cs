using System;
using System.Linq;
using Sitecore.SharedSource.Data;
using Sitecore.SharedSource.Web;
using Sitecore.Web.UI.HtmlControls;
using Page = System.Web.UI.Page;

namespace Sitecore.SharedSource
{
    public class BaseResourcePage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Sitecore.Context.Item == null || Sitecore.Context.Database == null)
            {
                return;
            }

            // Determine if the page has an addon.
            var pageItem = Sitecore.Context.Item;
            var resource = pageItem.Children.FirstOrDefault(x => x.TemplateID == TemplateIDs.ResourceReference);
            if (resource == null)
            {
                return;
            }

            var results = ResourceRenderer.LoadResourceReference(resource);

            if (results.Any(x => x.Resource == ResourceType.Script))
            {
                var placeholder = base.FindControl("ScriptsPlaceHolder");
                placeholder.Controls.Add(
                    new Literal(ResourceRenderer.RenderResourceHtmlString(results, ResourceType.Script)));
            }

            if (results.Any(x => x.Resource == ResourceType.Style))
            {
                var placeholder = base.FindControl("StylesPlaceHolder");
                placeholder.Controls.Add(
                    new Literal(ResourceRenderer.RenderResourceHtmlString(results, ResourceType.Style)));
            }
        }
    }
}