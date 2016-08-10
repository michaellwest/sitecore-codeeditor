using System.Collections.Generic;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SharedSource.Shell.Applications.ContentEditor;
using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.SharedSource.Shell.Framework.Commands.MenuItems
{
    public class EditCodeAsset : Command
    {
        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var item = context.Items[0];

            return IsMediaItem(item) ? (IsCodeItem(item) ? CommandState.Enabled : CommandState.Disabled) : CommandState.Hidden;
        }

        public bool IsCodeItem(Item item)
        {
            return item.TemplateID == TemplateIDs.UnversionedCode || item.TemplateID == TemplateIDs.VersionedCode;
        }

        public bool IsMediaItem(Item item)
        {
            return item.Paths.IsMediaItem;
        }

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var parameters = new NameValueCollection
            {
                {"id", context.Items[0].ID.ToString() },
                {"language", context.Items[0].Language.Name },
                {"version", context.Items[0].Version.ToString() }
            };

            Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(ClientPipelineArgs args)
        {
            var code = new CodeAttachment
            {
                ItemID = args.Parameters["id"],
                ItemLanguage = args.Parameters["language"],
                ItemVersion = args.Parameters["version"],
                FieldID = "Blob"
            };
            code.EditCode(args);
        }

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
                Header = "Edit Code",
                Icon = "Software/32x32/text_code.png",
                Click = $"codetext:editcode(id={item.ID})"
            };

            menuItems.Add(menuItem);
            menuItems.Add(new MenuDivider());

            return menuItems.ToArray();
        }
    }
}
