using Sitecore.SharedSource.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;
using Sitecore.Web.UI.HtmlControls;

namespace Sitecore.Sharedsource.Shell.Applications.ContentEditor.Pipelines
{
    public class SetEditorFormatter
    {
        public void Process(RenderContentEditorArgs args)
        {
            if (Registry.GetString("/Current_User/Content Editor/Translate") == "on")
            {
                args.EditorFormatter = new TranslatorFormatterExtended()
                {
                    Arguments = args
                };
            }
            else
            {
                // If another pipeline overrides the editor avoid breaking it.
                if (args.EditorFormatter == null || args.EditorFormatter.GetType() == typeof (EditorFormatter))
                {
                    args.EditorFormatter = new EditorFormatterExtended
                    {
                        Arguments = args
                    };
                }
            }
        }
    }
}