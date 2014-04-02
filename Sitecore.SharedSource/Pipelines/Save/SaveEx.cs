using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.Save;

namespace Sitecore.SharedSource.Pipelines.Save
{
    public class SaveEx
    {
        public void Process(SaveArgs args)
        {
            foreach (var saveItem in args.Items)
            {
                var item = Context.ContentDatabase.Items[saveItem.ID, saveItem.Language, saveItem.Version];
                if (item == null)
                {
                    continue;
                }
                if (item.Locking.IsLocked() && !item.Locking.HasLock() &&
                    (!Context.User.IsAdministrator && !args.PolicyBasedLocking))
                {
                    args.Error = "Could not modify locked item \"" + item.Name + "\"";
                    args.AbortPipeline();
                    return;
                }
                item.Editing.BeginEdit();
                foreach (var saveField in saveItem.Fields)
                {
                    var field = item.Fields[saveField.ID];
                    if (field == null)
                    {
                        continue;
                        ;
                    }

                    if (field.IsBlobField &&
                        ((field.TypeKey == "attachment" || field.TypeKey == "code attachment") ||
                         saveField.Value == Translate.Text("[Blob Value]")))
                    {
                        continue;
                    }

                    saveField.OriginalValue = field.Value;
                    if (saveField.OriginalValue == saveField.Value)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(saveField.Value) && NeedsHtmlTagEncode(saveField))
                    {
                        saveField.Value = saveField.Value.Replace("<", "&lt;").Replace(">", "&gt;");
                    }

                    field.Value = saveField.Value;
                }
                item.Editing.EndEdit();
                Log.Audit(this, "Save item: {0}", new[] {AuditFormatter.FormatItem(item)});
                args.SavedItems.Add(item);
            }
            if (!Context.IsUnitTesting)
            {
                Context.ClientPage.Modified = false;
            }

            if (!args.SaveAnimation)
            {
                return;
            }

            Context.ClientPage.ClientResponse.Eval("var d = new scSaveAnimation('ContentEditor')");
        }

        private static bool NeedsHtmlTagEncode(SaveArgs.SaveField field)
        {
            return field.ID == FieldIDs.DisplayName;
        }
    }
}