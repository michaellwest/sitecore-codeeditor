using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SharedSource.Configuration;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.SharedSource.Shell.Framework.Commands.MenuItems
{
    public class ItemNew : Sitecore.Shell.Framework.Commands.ItemNew
    {
        public override Control[] GetSubmenuItems(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var controls = base.GetSubmenuItems(context);

            if (controls == null || context.Items.Length != 1 || context.Items[0] == null)
            {
                return controls;
            }

            var item = context.Items[0];

            var menuItems = new List<Control>();
            menuItems.AddRange(controls);

            var menuItem = new MenuItem
            {
                Header = "Code Asset",
                Icon = "Software/32x32/text_code_add.png"
            };

            var submenuItems = new List<Control>();
            GetSubmenuItems(item, submenuItems);
            submenuItems.ForEach(x=> menuItem.Controls.Add(x));

            menuItems.Add(menuItem);
            menuItems.Add(new MenuDivider());
            
            return menuItems.ToArray();
        }

        public void GetSubmenuItems(Item contextItem, List<Control>  menuItems)
        {
            var settings = new MediaTypeSettings();

            foreach (var mediaType in settings.MediaTypes.OrderBy(x => x.Value.Name))
            {
                menuItems.Add(new MenuItem
                {
                    Header = Translate.Text(mediaType.Value.Name),
                    Icon = "Software/32x32/text_code_colored.png",
                    Click = $"codetext:newmediaitem(id={contextItem.ID},db={contextItem.Database.Name},ext={mediaType.Value.Extensions})"
                });
            }
        }
    }
}
