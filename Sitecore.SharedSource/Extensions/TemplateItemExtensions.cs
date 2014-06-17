using Sitecore.Data;
using Sitecore.Data.Items;

namespace Sitecore.SharedSource.Extensions
{
    public static class TemplateItemExtensions
    {
        public static bool InheritsFrom(this TemplateItem templateItem, ID templateId)
        {
            if (templateItem.ID == templateId)
            {
                return true;
            }

            foreach (var template in templateItem.BaseTemplates)
            {
                if (template.ID == templateId)
                {
                    return true;
                }

                if (template.InheritsFrom(templateId))
                {
                    return true;
                }
            }

            return false;
        }
    }
}