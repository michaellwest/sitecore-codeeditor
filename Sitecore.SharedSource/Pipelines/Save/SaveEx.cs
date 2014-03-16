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
                var obj = Context.ContentDatabase.Items[saveItem.ID, saveItem.Language, saveItem.Version];
                if (obj == null) continue;
                if (obj.Locking.IsLocked() && !obj.Locking.HasLock() &&
                    (!Context.User.IsAdministrator && !args.PolicyBasedLocking))
                {
                    args.Error = "Could not modify locked item \"" + obj.Name + "\"";
                    args.AbortPipeline();
                    return;
                }
                obj.Editing.BeginEdit();
                foreach (var field1 in saveItem.Fields)
                {
                    var field2 = obj.Fields[field1.ID];
                    if (field2 == null ||
                        (field2.IsBlobField &&
                         ((field2.TypeKey == "attachment" || field2.TypeKey == "code attachment") ||
                          field1.Value == Translate.Text("[Blob Value]")))) continue;

                    field1.OriginalValue = field2.Value;
                    if (field1.OriginalValue == field1.Value) continue;

                    if (!string.IsNullOrEmpty(field1.Value) && NeedsHtmlTagEncode(field1))
                    {
                        field1.Value = field1.Value.Replace("<", "&lt;").Replace(">", "&gt;");
                    }

                    field2.Value = field1.Value;
                }
                obj.Editing.EndEdit();
                Log.Audit(this, "Save item: {0}", new string[1]
                {
                    AuditFormatter.FormatItem(obj)
                });
                args.SavedItems.Add(obj);
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