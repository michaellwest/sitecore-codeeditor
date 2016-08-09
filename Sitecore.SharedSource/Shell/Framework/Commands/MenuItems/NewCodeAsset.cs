using System;
using System.IO;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.SharedSource.Shell.Framework.Commands.MenuItems
{
    public class NewCodeAsset : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            var parameters = new System.Collections.Specialized.NameValueCollection
            {
                ["id"] = context.Parameters["id"],
                ["db"] = context.Parameters["db"],
                ["ext"] = context.Parameters["ext"]
            };
            Context.ClientPage.Start(this, "Run", parameters);
        }

        protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.IsPostBack)
            {
                if ((String.IsNullOrEmpty(args.Result)) || args.Result == "undefined" || args.Result == "null") return;

                var name = args.Result;
                var db = args.Parameters["db"];
                var id = args.Parameters["id"];
                var ext = args.Parameters["ext"];
                AddMediaItem(id, db, name, ext);
            }
            else
            {

                Context.ClientPage.ClientResponse.Input(StringUtil.GetString(new string[2]
                {
                  args.Parameters["prompt"],
                  "Enter a name for the new item:"
                }), String.Empty, Settings.ItemNameValidation,
                "'$Input' is not a valid name.", Settings.MaxItemNameLength);
                args.WaitForPostBack();
            }
        }

        public MediaItem AddMediaItem(string parentId, string database, string name, string extension)
        {
            var db = Factory.GetDatabase(database);
            var parentItem = db.GetItem(parentId);
            var options = new Resources.Media.MediaCreatorOptions
            {
                FileBased = false,
                IncludeExtensionInItemName = false,
                Versioned = false,
                Destination = parentItem.Paths.Path + "/" + name,
                Database = db
            };

            var creator = new Resources.Media.MediaCreator();
            var mediaItem = creator.CreateFromStream(new MemoryStream(new byte[] { 00 }), $"{name}.{extension}", options);
            return mediaItem;
        }
    }
}
